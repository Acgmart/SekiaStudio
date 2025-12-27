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
            root.AddComponent<ResourcesLoaderComponent>();
            root.AddComponent<MyPlayerComponent>();
            root.AddComponent<CurrentScenesComponent>();
            
            World.Instance.AddSingleton<FUIEventComponent>();
            root.AddComponent<FUIAssetComponent, bool>(false);
            root.AddComponent<FUIComponent>();

            await EventSystem.Instance.PublishAsync(root, new AppStartInitFinish());
        }
    }
}