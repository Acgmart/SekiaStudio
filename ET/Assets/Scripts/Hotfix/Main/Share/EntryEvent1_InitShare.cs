
namespace ET
{
    [Publish(SceneType.StateSync)]
    public class EntryEvent1_InitShare : APublishHandler<EntryEvent1>
    {
        protected override async ETTask Run(Scene root, EntryEvent1 args)
        {
            root.AddComponent<TimerComponent>();
            root.AddComponent<CoroutineLockComponent>();
            root.AddComponent<ObjectWait>();
            root.AddComponent<MailBoxComponent, int>(MailBoxType.UnOrderedMessage);
            root.AddComponent<ProcessInnerSender>();
            
            await ETTask.CompletedTask;
        }
    }
}