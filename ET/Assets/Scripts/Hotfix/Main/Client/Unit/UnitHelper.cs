namespace ET.Client
{
    public static partial class UnitHelper
    {
        public static Unit GetMyUnitFromClientScene(Scene root)
        {
            MyPlayerComponent playerComponent = root.GetComponent<MyPlayerComponent>();
            Scene currentScene = root.GetComponent<CurrentScenesComponent>().Scene;
            return currentScene.GetComponent<UnitComponent>().Get(playerComponent.MyId);
        }
        
        public static Unit GetMyUnitFromCurrentScene(Scene currentScene)
        {
            MyPlayerComponent playerComponent = currentScene.Root().GetComponent<MyPlayerComponent>();
            return currentScene.GetComponent<UnitComponent>().Get(playerComponent.MyId);
        }
    }
}