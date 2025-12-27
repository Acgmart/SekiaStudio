namespace ET.Client
{
    [Publish(SceneType.StateSync)]
    public class AppStartInitFinish_CreateLoginUI : APublishHandler<AppStartInitFinish>
    {
        protected override async ETTask Run(Scene root, AppStartInitFinish args)
        {
            //首次加载UI组件
            FUIComponent fuiComponent = root.GetComponent<FUIComponent>();
            fuiComponent.Restart();
            
            // 打开登陆界面
            LoginPanel_ContextData contextData = fuiComponent.AddChild<LoginPanel_ContextData>();
            contextData.Data = "界面参数测试";
            // 显示登录界面, 并传递参数contextData
            await fuiComponent.ShowPanelAsync(PanelId.LoginPanel, contextData);
        }
    }
}
