using System;
using ET.Client;
using FairyGUI;

namespace ET.Client
{
    [FriendOf(typeof(GlobalComponent))]
    public static class FUIRootHelper
    {
        public static void Init()
        {
          
        }
        
        public static GComponent GetTargetRoot(this GlobalComponent self, UIPanelType type)
        {
            if (type == UIPanelType.Normal)
            {
                return self.NormalGRoot;
            }
            else if (type == UIPanelType.Fixed)
            {
                return self.FixedGRoot;
            }
            else if (type == UIPanelType.PopUp)
            {
                return self.PopUpGRoot;
            }
            else if (type == UIPanelType.Other)
            {
                return self.OtherGRoot;
            }

            Log.Error("uiroot type is error: " + type.ToString());
            return null;
        }
    }
}