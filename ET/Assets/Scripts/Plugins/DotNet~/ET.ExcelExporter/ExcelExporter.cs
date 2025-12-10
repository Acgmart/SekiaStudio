using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace ET
{
    class HeadInfo
    {
        [BsonElement]
        public string FieldDesc;
        public string FieldName;
        public string FieldType;
        public int FieldIndex;

        public HeadInfo(string desc, string name, string type, int index)
        {
            this.FieldDesc = desc;
            this.FieldName = name;
            this.FieldType = type;
            this.FieldIndex = index;
        }
    }

    // 这里加个标签是为了防止编译时裁剪掉protobuf，因为整个tool工程没有用到protobuf，编译会去掉引用，然后动态编译就会出错
    class Table
    {
        public string Name;
        public int Index;
        public Dictionary<string, HeadInfo> HeadInfos = new();
    }
    
    [EnableClass]
    public static class ExcelExporter
    {
        private const string excelPath = "Assets/Res/Excel";
        private static string template;
        private const string excelConfigExportDir = "./Assets/Scripts/Model/ExcelConfig";
        private const string jsonDir = "./Assets/Res/Config/Json";
        private const string bytesDir = "./Assets/Res/Config/Bytes";
        private static Assembly configAssemblie = null;

        private static Dictionary<string, Table> tables = new();
        private static Dictionary<string, ExcelPackage> packages = new();

        private static Table GetTable(string protoName)
        {
            string fullName = protoName;
            if (!tables.TryGetValue(fullName, out var table))
            {
                table = new Table();
                table.Name = protoName;
                tables[fullName] = table;
            }

            return table;
        }

        public static ExcelPackage GetPackage(string filePath)
        {
            if (!packages.TryGetValue(filePath, out var package))
            {
                using Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                package = new ExcelPackage(stream);
                packages[filePath] = package;
            }

            return package;
        }

        public static void Export()
        {
            //避免裁剪
            Console.WriteLine(MongoDB.Bson.BsonString.Empty);

            //设置当前路径为Unity工程根目录
            string currentDir = Directory.GetCurrentDirectory();
            if (currentDir.EndsWith("Assets\\Scripts\\Plugins\\DotNet~\\ET.ExcelExporter\\Exe"))
            {
                currentDir = currentDir.Substring(0, currentDir.IndexOf("Assets"));
                Directory.SetCurrentDirectory(currentDir);
            }

            try
            {
                template = File.ReadAllText("./Assets/Scripts/Plugins/DotNet~/ET.ExcelExporter/Template.txt");
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                
                if (Directory.Exists(jsonDir))
                    Directory.Delete(jsonDir, true);
                Directory.CreateDirectory(jsonDir);
                if (Directory.Exists(bytesDir))
                    Directory.Delete(bytesDir, true);
                Directory.CreateDirectory(bytesDir);

                //.xlsm格式支持保存宏代码
                List<string> paths = new();
                foreach (string k in FileHelper.GetAllFiles(excelPath))
                    if ((k.EndsWith(".xlsx") || k.EndsWith(".xlsm")) &&
                        !k.StartsWith("~$") &&
                        !k.Contains("#"))
                        paths.Add(k);

                //生成.cs文件
                {
                    foreach (string path in paths)
                    {
                        ExcelPackage p = GetPackage(Path.GetFullPath(path));
                        string fileName = Path.GetFileName(path);
                        string protoName = Path.GetFileNameWithoutExtension(fileName);
                        Table table = GetTable(protoName);
                        ExportExcelClass(p, protoName, table);
                    }

                    foreach (var kv in tables)
                    {
                        ExportClass(kv.Value);
                    }
                }

                //动态编译
                configAssemblie = DynamicBuild();
                
                //导出配置
                foreach (string path in paths)
                    ExportExcel(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                tables.Clear();
                foreach (var kv in packages)
                {
                    kv.Value.Dispose();
                }
                packages.Clear();
            }
        }

        private static void ExportExcel(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string relativePath = Path.GetRelativePath(excelPath, dir);
            string protoName = Path.GetFileNameWithoutExtension(path);
            Table table = GetTable(protoName);
            ExcelPackage p = GetPackage(Path.GetFullPath(path));
            ExportExcelJson(p, protoName, table, relativePath);
            ExportExcelProtobuf(table, relativePath);
        }

        // 动态编译生成的cs代码
        private static Assembly DynamicBuild()
        {
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            List<string> protoNames = new List<string>();
            foreach (string classFile in FileHelper.GetAllFiles(excelConfigExportDir, "*.cs"))
            {
                protoNames.Add(Path.GetFileNameWithoutExtension(classFile));
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(classFile)));
            }

            List<PortableExecutableReference> references = new List<PortableExecutableReference>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                //包括当前程序集、所有依赖项(包、项目、引用)、DOTNET运行时
                //即对当前程序集有完整的引用
                //可以直接把生成的代码放进来看是否报错
                try
                {
                    if (assembly.IsDynamic)
                    {
                        continue;
                    }

                    if (assembly.Location == "")
                    {
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                PortableExecutableReference reference = MetadataReference.CreateFromFile(assembly.Location);
                references.Add(reference);
            }
            CSharpCompilation compilation = CSharpCompilation.Create(null,
                syntaxTrees.ToArray(),
                references.ToArray(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using MemoryStream memSteam = new MemoryStream();
            EmitResult emitResult = compilation.Emit(memSteam);
            if (!emitResult.Success)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Diagnostic t in emitResult.Diagnostics)
                {
                    stringBuilder.Append($"{t.GetMessage()}\n");
                }

                throw new Exception($"动态编译失败:\n{stringBuilder}");
            }

            memSteam.Seek(0, SeekOrigin.Begin);

            Assembly ass = Assembly.Load(memSteam.ToArray());
            return ass;
        }


        #region 生成.cs文件
        //const和enum直接生成.cs文件 class先收集字段
        static void ExportExcelClass(ExcelPackage p, string name, Table table)
        {
            foreach (ExcelWorksheet worksheet in p.Workbook.Worksheets)
            {
                string sheetName = worksheet.Name.ToLower();
                if (sheetName.StartsWith("#const_"))
                {
                    ExportConstClass(worksheet);
                    continue;
                }
                if (sheetName.StartsWith("#enum_"))
                {
                    ExportEnumClass(worksheet);
                    continue;
                }

                ExportSheetClass(worksheet, table);
            }
        }

        //收集表格字段
        static void ExportSheetClass(ExcelWorksheet worksheet, Table table)
        {
            const int row = 2;
            for (int col = 3; col <= worksheet.Dimension.End.Column; ++col)
            {
                if (worksheet.Name.StartsWith("#"))
                {
                    continue;
                }

                string fieldName = worksheet.Cells[row + 2, col].Text.Trim();
                if (fieldName == "")
                {
                    continue;
                }

                if (table.HeadInfos.ContainsKey(fieldName))
                {
                    continue;
                }

                string fieldTag = worksheet.Cells[row, col].Text.Trim().ToLower();
                if (fieldTag.Contains("#")) //注释
                {
                    table.HeadInfos[fieldName] = null;
                    continue;
                }
                
                string fieldDesc = worksheet.Cells[row + 1, col].Text.Trim();
                string fieldType = worksheet.Cells[row + 3, col].Text.Trim();

                table.HeadInfos[fieldName] = new HeadInfo(fieldDesc, fieldName, fieldType, ++table.Index);
            }
        }
        
        //生成.cs文件
        static void ExportClass(Table table)
        {
            string exportPath = Path.Combine(excelConfigExportDir, $"{table.Name}.cs");

            if (!Directory.Exists(Path.GetDirectoryName(exportPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(exportPath));
            }
            
            using FileStream txt = new FileStream(exportPath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(txt);

            StringBuilder sb = new StringBuilder();
            foreach ((string _, HeadInfo headInfo) in table.HeadInfos)
            {
                if (headInfo == null)
                {
                    continue;
                }

                sb.Append($"\t\t/// <summary>{headInfo.FieldDesc}</summary>\n");
                string fieldType = headInfo.FieldType;
                sb.Append($"\t\tpublic {fieldType} {headInfo.FieldName} {{ get; set; }}\n");
            }

            //template = template.Replace("(ns)", $"ET.{table.Module}");
            template = template.Replace("(ns)", "ET");
            string content = template.Replace("(ConfigName)", table.Name).Replace(("(Fields)"), sb.ToString());
            sw.Write(content);
        }

        //生成.cs文件
        static void ExportConstClass(ExcelWorksheet worksheet)
        {
            const int row = 2;
            List<string> listConst = new List<string>();
            
            for (int col = 3; col <= worksheet.Dimension.End.Column; ++col)
            {
                string fieldName = worksheet.Cells[row + 2, col].Text.Trim();
                if (fieldName == "")
                {
                    continue;
                }

                string fieldTag = worksheet.Cells[row, col].Text.Trim().ToLower();
                if (fieldTag.Contains('#'))
                {
                    continue;
                }
                
                string fieldType = worksheet.Cells[row + 3, col].Text.Trim();

                if (fieldType.ToLower() == "const")
                {
                    string constType = worksheet.Cells[row + 3, col + 1].Text.Trim();
                    
                    //数组类型 ,需要使用static readonly
                    bool isStatic = constType.Contains("[]");
                    
                    for (int i = 0; i < 999999; i++)
                    {
                        string name = worksheet.Cells[row + 4 + i, col].Text.Trim();
                        if(string.IsNullOrEmpty(name)) break;
                        
                        string desc = worksheet.Cells[row + 4 + i, col - 1].Text.Trim();
                        string val = worksheet.Cells[row + 4 + i, col + 1].Text.Trim();

                        if (isStatic)
                        {
                            //数组类型,需要使用{}包裹
                            val = "{" + val + "}";
                            listConst.Add($"        /// <summary>{desc}</summary>\n        [StaticField]\n        public static readonly {constType} {name} = {val}; \n");
                        }
                        else
                        {
                            listConst.Add($"        /// <summary>{desc}</summary>\n        public const {constType} {name} = {Convert(constType, val)}; \n");
                        }
                    }
                }
            }
            
            
            string cs = worksheet.Cells[1, 1].Text.Trim();
            
            string ename = worksheet.Name.Substring(7); // #const_ 7个字符
            string exportPath = Path.Combine(excelConfigExportDir, $"{ename}.cs");

            using FileStream txt = new FileStream(exportPath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(txt);

            //生成常量
            sw.WriteLine("namespace ET");
            sw.WriteLine("{");
            sw.WriteLine($"    public static partial class {ename}");
            sw.WriteLine("    {");
            for (int i = 0; i < listConst.Count; i++)
            {
                sw.WriteLine(listConst[i]);
            }
            sw.WriteLine("    }");
            sw.WriteLine("}");
        }

        //生成.cs文件
        static void ExportEnumClass(ExcelWorksheet worksheet)
        {
            const int row = 2;
            List<string> listEnums = new List<string>();
            
            for (int col = 3; col <= worksheet.Dimension.End.Column; ++col)
            {
                string fieldName = worksheet.Cells[row + 2, col].Text.Trim();
                if (fieldName == "")
                {
                    continue;
                }

                string fieldTag = worksheet.Cells[row, col].Text.Trim().ToLower();
                if (fieldTag.Contains('#'))
                {
                    continue;
                }
                
                string fieldType = worksheet.Cells[row + 3, col].Text.Trim();

                if (fieldType.ToLower() == "enum")
                {
                    for (int i = 0; i < 999999; i++)
                    {
                        string name = worksheet.Cells[row + 4 + i, col].Text.Trim();
                        if(string.IsNullOrEmpty(name)) break;
                        
                        string desc = worksheet.Cells[row + 4 + i, col - 1].Text.Trim();
                        string val = worksheet.Cells[row + 4 + i, col + 1].Text.Trim();

                        if (string.IsNullOrEmpty(val)) 
                            val = ",";
                        else
                        {
                            val = $" = {val},";
                        }
                        
                        listEnums.Add($"        /// <summary>{desc}</summary>\n        {name}{val}\n");
                    }
                }
            }
            
            
            string cs = worksheet.Cells[1, 1].Text.Trim();

            string ename = worksheet.Name.Substring(6); // #enum_ 6个字符
            string exportPath = Path.Combine(excelConfigExportDir, $"{ename}.cs");

            using FileStream txt = new FileStream(exportPath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(txt);

            //生成枚举
            sw.WriteLine("namespace ET");
            sw.WriteLine("{");
            sw.WriteLine($"    public enum {ename}");
            sw.WriteLine("    {");
            for (int i = 0; i < listEnums.Count; i++)
            {
                sw.WriteLine(listEnums[i]);
            }
            sw.WriteLine("    }");
            sw.WriteLine("}");
        }
        #endregion

        #region 导出json

        static void ExportExcelJson(ExcelPackage p, string name, Table table, string relativeDir)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"dict\": [\n");
            foreach (ExcelWorksheet worksheet in p.Workbook.Worksheets)
            {
                if (worksheet.Name.StartsWith("#"))
                {
                    continue;
                }

                ExportSheetJson(worksheet, name, table, sb);
            }

            sb.Append("]}\n");

            string dir = Path.Combine(jsonDir, relativeDir);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string jsonPath = Path.Combine(dir, $"{name}.txt");
            using FileStream txt = new FileStream(jsonPath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(txt);
            sw.Write(sb.ToString());
        }

        static void ExportSheetJson(ExcelWorksheet worksheet, string name, Table table, StringBuilder sb)
        {
            for (int row = 6; row <= worksheet.Dimension.End.Row; ++row)
            {
                string prefix = worksheet.Cells[row, 2].Text.Trim();
                if (prefix.Contains("#"))
                {
                    continue;
                }

                if (worksheet.Cells[row, 3].Text.Trim() == "")
                {
                    continue;
                }

                sb.Append($"[{worksheet.Cells[row, 3].Text.Trim()}, {{\"_t\":\"{name}\"");
                for (int col = 3; col <= worksheet.Dimension.End.Column; ++col)
                {
                    string fieldName = worksheet.Cells[4, col].Text.Trim();
                    if (!table.HeadInfos.ContainsKey(fieldName))
                    {
                        continue;
                    }

                    HeadInfo headInfo = table.HeadInfos[fieldName];

                    if (headInfo == null)
                    {
                        continue;
                    }

                    string fieldN = headInfo.FieldName;
                    if (fieldN == "Id")
                    {
                        fieldN = "_id";
                    }

                    sb.Append($",\"{fieldN}\":{Convert(headInfo.FieldType, worksheet.Cells[row, col].Text.Trim())}");
                }

                sb.Append("}],\n");
            }
        }

        #endregion

        //处理类型默认值
        private static string Convert(string type, string value)
        {
            switch (type)
            {
                case "uint[]":
                case "int[]":
                case "int32[]":
                case "long[]":
                    if (string.IsNullOrEmpty(value))
                        return "[0]";

                    return $"[{value}]";
                case "string[]":
                case "int[][]":
                    return $"[{value}]";
                case "int":
                case "uint":
                case "int32":
                case "int64":
                case "long":
                case "float":
                case "double":
                    if (value == "")
                    {
                        return "0";
                    }

                    return value;
                case "string":
                    value = value.Replace("\\", "\\\\");
                    value = value.Replace("\"", "\\\"");
                    return $"\"{value}\"";
                case "bool":
                    {
                        if (value == "1")
                            return "true";
                        if (value == "0" || string.IsNullOrEmpty(value))
                            return "false";

                        return value;
                    }
                default:
                    throw new Exception($"不支持此类型: {type}");
            }
        }

        // 根据生成的类，把json转成protobuf
        private static void ExportExcelProtobuf(Table table, string relativeDir)
        {
            string dir = Path.Combine(bytesDir, relativeDir);
            string moduleDir = Path.Combine(dir);
            if (!Directory.Exists(moduleDir))
            {
                Directory.CreateDirectory(moduleDir);
            }

            Type type = configAssemblie.GetType($"ET.{table.Name}Category");

            IMerge final = Activator.CreateInstance(type) as IMerge;

            string p = Path.Combine(jsonDir, relativeDir);
            string[] ss = Directory.GetFiles(p, $"{table.Name}*.txt");
            List<string> jsonPaths = ss.ToList();

            jsonPaths.Sort();
            jsonPaths.Reverse();
            foreach (string jsonPath in jsonPaths)
            {
                string json = File.ReadAllText(jsonPath);
                try
                {
                    object deserialize = BsonSerializer.Deserialize(json, type);
                    final.Merge(deserialize);
                }
                catch (Exception e)
                {
                    throw new Exception($"json : {jsonPath} error", e);
                }
            }

            string path = Path.Combine(moduleDir, $"{table.Name}Category.bytes");

            using FileStream file = File.Create(path);
            file.Write(final.ToBson());
        }
    }
}
