namespace ET.Client
{
    [Publish(SceneType.StateSync)]
    public class AfterCreateClientScene_AddComponent : APublishHandler<AfterCreateClientScene>
    {
        protected override async ETTask Run(Scene scene, AfterCreateClientScene args)
        {
            scene.AddComponent<UIComponent>();
            scene.AddComponent<ResourcesLoaderComponent>();
            await ETTask.CompletedTask;
        }
    }
}