using UnityEditor;
using UnityEngine;
using FUIEditor;

namespace ET
{
    public static class InitHelper
    {
        [MenuItem("ET/Init/InitAll", false, 0)]
        public static void Init()
        {
            ExcelEditor.Init();
            
            ProtoEditor.Init();
            
            FUICodeSpawner.FUICodeSpawn();
            
            DefineHelper.EnableDefineSymbols("INITED", true);

            AssetDatabase.Refresh();
            
            Debug.Log("Init finish!");
        }
    }
}