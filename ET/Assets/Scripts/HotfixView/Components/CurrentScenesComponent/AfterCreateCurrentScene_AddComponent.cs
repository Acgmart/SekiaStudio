namespace ET.Client
{
    [Publish(SceneType.Current)]
    public class AfterCreateCurrentScene_AddComponent : APublishHandler<AfterCreateCurrentScene>
    {
        protected override async ETTask Run(Scene scene, AfterCreateCurrentScene args)
        {
            scene.AddComponent<ResourcesLoaderComponent>();
            scene.AddComponent<FUIComponent>();
            
            await ETTask.CompletedTask;
        }
    }
}