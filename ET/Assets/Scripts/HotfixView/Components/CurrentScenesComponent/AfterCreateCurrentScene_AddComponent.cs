namespace ET.Client
{
    [Publish(SceneType.Current)]
    public class AfterCreateCurrentScene_AddComponent : APublishHandler<AfterCreateCurrentScene>
    {
        protected override async ETTask Run(Scene scene, AfterCreateCurrentScene args)
        {
            scene.AddComponent<UIComponent>();
            scene.AddComponent<ResourcesLoaderComponent>();
            await ETTask.CompletedTask;
        }
    }
}