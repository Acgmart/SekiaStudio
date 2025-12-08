using UnityEditor;
using UnityEngine;

namespace ET
{
    public static class InitHelper
    {
        [MenuItem("ET/StateSync/Init")]
        public static void Init()
        {
            ExcelEditor.Init();
            
            ProtoEditor.Init();
            
            DefineHelper.EnableDefineSymbols("INITED", true);

            AssetDatabase.Refresh();
            
            Debug.Log("Init finish!");
        }
    }
}