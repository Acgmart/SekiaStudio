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
            CodeModeChangeHelper.ChangeToCodeMode("ClientServer");
            
            InitScriptHelper.Run();
            
            DefineHelper.EnableDefineSymbols("INITED", true);
#endif
        }
    }
}