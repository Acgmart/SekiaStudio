using System;
using System.Collections.Generic;

namespace ET.Client
{
    public struct PanelInfo
    {
        public PanelId PanelId;
    
        public string PackageName;
    
        public string ComponentName;
    }
    
    public class FUIEventComponent : Singleton<FUIEventComponent>, ISingletonAwake
    {
        public readonly Dictionary<PanelId, IFUIEventHandler> UIEventHandlers = new ();
        public readonly Dictionary<PanelId, PanelInfo> PanelIdInfoDict = new ();
        public readonly Dictionary<string, PanelInfo> PanelTypeInfoDict = new ();

        public bool isClicked { get; set; }
        
        public void Awake()
        {
            this.UIEventHandlers.Clear();
            this.PanelIdInfoDict.Clear();
            this.PanelTypeInfoDict.Clear();

            var uiEvents = CodeTypes.Instance.GetTypes(typeof(FUIEventAttribute));
            foreach (Type type in uiEvents)
            {
                object[] attrs = type.GetCustomAttributes(typeof (FUIEventAttribute), false);
                if (attrs.Length == 0)
                    continue;
                FUIEventAttribute attr = attrs[0] as FUIEventAttribute;
                this.UIEventHandlers.Add(attr.PanelId, Activator.CreateInstance(type) as IFUIEventHandler);
                this.PanelIdInfoDict.Add(attr.PanelId, attr.PanelInfo);
                this.PanelTypeInfoDict.Add(attr.PanelId.ToString(), attr.PanelInfo);
            }
        }
        
        public IFUIEventHandler GetUIEventHandler(PanelId panelId)
        {
            if (this.UIEventHandlers.TryGetValue(panelId, out IFUIEventHandler handler))
            {
                return handler;
            }
            Log.Error($"panelId : {panelId} is not have any uiEvent");
            return null;
        }

        public PanelInfo GetPanelInfo(PanelId panelId)
        {
            if (this.PanelIdInfoDict.TryGetValue(panelId, out PanelInfo panelInfo))
            {
                return panelInfo;
            }
            Log.Error($"panelId : {panelId} is not have any panelInfo");
            return default;
        }
    }
}