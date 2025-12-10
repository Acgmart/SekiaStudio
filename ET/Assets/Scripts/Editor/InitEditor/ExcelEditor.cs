using UnityEditor;
using System.Diagnostics;
using System.IO;

namespace ET
{
    public static class ExcelEditor
    {
        [MenuItem("ET/Init/ExcelExporter")]
        public static void Run()
        {
            if(!File.Exists("./Assets/Scripts/Plugins/DotNet~/ET.ExcelExporter/Exe/ET.ExcelExporter.dll"))
                ProcessHelper.DotNet("build --configuration Release", "./Assets/Scripts/Plugins/DotNet~/ET.ExcelExporter/", true);
            Process process = ProcessHelper.DotNet("./Assets/Scripts/Plugins/DotNet~/ET.ExcelExporter/Exe/ET.ExcelExporter.dll", "./", true);

            UnityEngine.Debug.Log(process.StandardOutput.ReadToEnd());
        }

        public static void Init()
        {
            Run();
        }
    }
}