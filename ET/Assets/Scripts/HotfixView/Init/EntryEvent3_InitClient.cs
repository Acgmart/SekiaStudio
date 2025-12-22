using System;
using System.Collections.Generic;
using System.IO;

namespace ET.Client
{
    [Publish(SceneType.StateSync)]
    public class EntryEvent3_InitClient : APublishHandler<EntryEvent3>
    {
        protected override async ETTask Run(Scene root, EntryEvent3 args)
        {
            root.AddComponent<GlobalComponent>();
            root.AddComponent<UIGlobalComponent>();
            root.AddComponent<UIComponent>();
            root.AddComponent<ResourcesLoaderComponent>();
            root.AddComponent<MyPlayerComponent>();
            root.AddComponent<CurrentScenesComponent>();

            await EventSystem.Instance.PublishAsync(root, new AppStartInitFinish());
        }
    }
}