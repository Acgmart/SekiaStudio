using UnityEditor;

namespace ET
{
    public static class LoaderEditor
    {
        [MenuItem("ET/Loader/Init")]
        public static void Init()
        {
#if INITED
            UnityEngine.Debug.LogError("Your project are already inited, if you want to reinit, please remove INITED define in unity!");
#else
            GlobalConfig globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>("Packages/cn.etetet.loader/Resources/GlobalConfig.asset");
            CodeModeChangeHelper.ChangeToCodeMode(globalConfig.CodeMode.ToString()); //Client/Server/ClientServer
            
            InitScriptHelper.Run();
            
            DefineHelper.EnableDefineSymbols("INITED", true);
#endif
        }
    }
}