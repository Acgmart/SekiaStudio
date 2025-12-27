namespace ET.Client
{
    //1.初次进入场景or场景切换-创建CurrenScene
    public struct AfterCreateCurrentScene
    {
    }
    
    //2.场景切换-加载场景资源
    public struct SceneChangeStart
    {
    }
    
    //3.场景切换-加载Map场景UI
    public struct SceneChangeFinish
    {
    }
    
    //场景切换-完成
    public struct EnterMapFinish
    {
    }
    
    //初始化完成-加载登陆UI
    public struct AppStartInitFinish
    {
    }
    
    //登陆完成-加载大厅UI-卸载登陆UI
    public struct LoginFinish
    {
    }

    public struct AfterUnitCreate
    {
        public Unit Unit;
    }
}