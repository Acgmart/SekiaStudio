using Unity.Mathematics;

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

            MongoRegister.RegisterStruct<float2>();
            MongoRegister.RegisterStruct<float3>();
            MongoRegister.RegisterStruct<float4>();
            MongoRegister.RegisterStruct<quaternion>();

            await ETTask.CompletedTask;
        }
    }
}