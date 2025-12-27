using FairyGUI;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(FUIEntity))]
    public static partial class FUIEntitySystem
    {
        [EntitySystem]
        private static void Awake(this FUIEntity self)
        {
            self.PanelCoreData = self.AddChild<PanelCoreData>();
        }
        
        [EntitySystem]
        private static void Destroy(this FUIEntity self)
        {
            self.PanelCoreData?.Dispose();
            self.PanelId = PanelId.Invalid;
            if (self.GComponent != null)
            {
                self.GComponent.Dispose();
                self.GComponent = null;
            }
        }
        
        public static void SetRoot(this FUIEntity self, GComponent rootGComponent)
        {
            if(self.GComponent == null)
            {
                Log.Error($"FUIEntity {self.PanelId} GComponent is null!!!");
                return;
            }
            if(rootGComponent == null)
            {
                Log.Error($"FUIEntity {self.PanelId} rootGComponent is null!!!");
                return;
            }
            rootGComponent.AddChild(self.GComponent);
        }
    }
    
    [ChildOf(typeof(FUIComponent))]
    public class FUIEntity : Entity, IAwake, IDestroy
    {
        public bool IsPreLoad
        {
            get
            {
                return this.GComponent != null;
            }
        }
        
        public PanelId PanelId
        {
            get
            {
                if (this.panelId == PanelId.Invalid)
                {
                    Log.Error("panel id is " + PanelId.Invalid);
                }
                return this.panelId;
            }
            set { this.panelId = value; }
        }
      
        private PanelId panelId = PanelId.Invalid;

        public GComponent GComponent { get; set; }

        public PanelCoreData PanelCoreData { get; set; }

        public EntityRef<Entity> ContextData { get; set; }

        public bool IsUsingStack;
    }
}