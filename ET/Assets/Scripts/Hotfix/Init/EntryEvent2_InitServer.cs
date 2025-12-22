namespace ET.Server
{
    [Publish(SceneType.StateSync)]
    public class EntryEvent2_InitServer : APublishHandler<EntryEvent2>
    {
        protected override async ETTask Run(Scene root, EntryEvent2 args)
        {
            await FiberManager.Instance.Create(SchedulerType.ThreadPool, SceneType.Gate, SceneType.Gate, "Gate");
        }
    }
}