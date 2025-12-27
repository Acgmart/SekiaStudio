using UnityEngine;
using FairyGUI;

namespace ET
{
    [EntitySystemOf(typeof(GlobalComponent))]
    public static partial class GlobalComponentSystem
    {
        [EntitySystem]
        private static void Awake(this GlobalComponent self)
        {
            self.Unit = GameObject.Find("/Global/Unit").transform;
            self.GlobalConfig = Resources.Load<GlobalConfig>("GlobalConfig");
            
            #region 初始化FairyGUI挂点
            
            self.GRoot = GRoot.inst;
            
            self.NormalGRoot = new GComponent();
            self.NormalGRoot.gameObjectName = "NormalGRoot";
            GRoot.inst.AddChild(self.NormalGRoot);
            
            self.PopUpGRoot = new GComponent();
            self.PopUpGRoot.gameObjectName = "PopUpGRoot";
            GRoot.inst.AddChild(self.PopUpGRoot);
            
            self.FixedGRoot = new GComponent();
            self.FixedGRoot.gameObjectName = "FixedGRoot";
            GRoot.inst.AddChild(self.FixedGRoot);
            
            self.OtherGRoot = new GComponent();
            self.OtherGRoot.gameObjectName = "OtherGRoot";
            GRoot.inst.AddChild(self.OtherGRoot);
            
            #endregion
        }
    }
    
    [ComponentOf(typeof(Scene))]
    public class GlobalComponent: Entity, IAwake
    {
        public Transform Unit { get; set; }

        public GlobalConfig GlobalConfig { get; set; }
        
        public GComponent GRoot{ get; set; }
        public GComponent NormalGRoot{ get; set; }
        public GComponent PopUpGRoot{ get; set; }
        public GComponent FixedGRoot{ get; set; }
        public GComponent OtherGRoot{ get; set; }
    }
}