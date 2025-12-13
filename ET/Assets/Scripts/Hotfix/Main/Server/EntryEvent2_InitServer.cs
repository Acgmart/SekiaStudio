namespace ET.Server
{
    [Publish(SceneType.StateSync)]
    public class EntryEvent2_InitServer : APublishHandler<EntryEvent2>
    {
        protected override async ETTask Run(Scene root, EntryEvent2 args)
        {
            World.Instance.AddSingleton<NavmeshComponent>();

            int process = 1;
            StartProcessConfig startProcessConfig = StartProcessConfigCategory.Instance.Get(process);
            if (startProcessConfig.Port != 0)
            {
                await FiberManager.Instance.Create(SchedulerType.ThreadPool, SceneType.NetInner, SceneType.NetInner, "NetInner");
            }

            // 根据配置创建纤程
            var scenes = StartSceneConfigCategory.Instance.GetByProcess(process);

            foreach (StartSceneConfig startConfig in scenes)
            {

                int sceneType = SceneTypeSingleton.Instance.GetSceneType(startConfig.SceneType);
                await FiberManager.Instance.Create(SchedulerType.ThreadPool, startConfig.Id, sceneType, startConfig.Name);
            }
        }
    }
}