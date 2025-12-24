namespace ET.Client
{
    [Publish(SceneType.StateSync)]
    public class AppStartInitFinish_CreateLoginUI : APublishHandler<AppStartInitFinish>
    {
        protected override async ETTask Run(Scene root, AppStartInitFinish args)
        {
            await UIHelper.Create(root, UIType.UILogin, UILayer.Mid);
        }
    }
}
