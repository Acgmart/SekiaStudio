
namespace ET.Client
{
    [Publish(SceneType.StateSync)]
    public class LoginFinish_CreateLobbyUI: APublishHandler<LoginFinish>
    {
        protected override async ETTask Run(Scene scene, LoginFinish a)
        {
            FUIComponent fuiComponent = scene.GetComponent<FUIComponent>();
            fuiComponent.HidePanel(PanelId.LoginPanel);
            await fuiComponent.ShowPanelAsync(PanelId.LobbyPanel);
        }
    }
}

