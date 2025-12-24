namespace ET.Client
{
    [Publish(SceneType.StateSync)]
    public class LoginFinish_CreateLobbyUI : APublishHandler<LoginFinish>
    {
        protected override async ETTask Run(Scene scene, LoginFinish args)
        {
            await UIHelper.Create(scene, UIType.UILobby, UILayer.Mid);
        }
    }
}
