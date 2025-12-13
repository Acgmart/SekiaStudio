namespace ET.Client
{
    [Publish(SceneType.StateSync)]
    public class LoginFinish_RemoveLoginUI : APublishHandler<LoginFinish>
    {
        protected override async ETTask Run(Scene scene, LoginFinish args)
        {
            await UIHelper.Remove(scene, UIType.UILogin);
        }
    }
}
