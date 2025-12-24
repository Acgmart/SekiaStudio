namespace ET.Client
{
    public static class CurrentScenesComponentSystem
    {
        public static Scene Create(this CurrentScenesComponent currentScenesComponent, long id, string name)
        {
            Scene currentScene = currentScenesComponent.CreateCurrentScene(id, IdGenerater.Instance.GenerateInstanceId(), SceneType.Current, name);
            currentScenesComponent.Scene = currentScene;
            //添加HotfixView组件
            EventSystem.Instance.Publish(currentScene, new AfterCreateCurrentScene());
            return currentScene;
        }
        
        private static Scene CreateCurrentScene(this Entity parent, long id, long instanceId, int sceneType, string name)
        {
            Scene scene = new(parent.Fiber(), id, instanceId, sceneType, name);
            parent?.AddChild(scene);
            return scene;
        }
        
        public static Scene CurrentScene(this Scene root)
        {
            return root.GetComponent<CurrentScenesComponent>()?.Scene;
        }
    }
    
    // 可以用来管理多个客户端场景，比如大世界会加载多块场景
    [ComponentOf(typeof(Scene))]
    public class CurrentScenesComponent: Entity, IAwake
    {
        private EntityRef<Scene> scene;

        public Scene Scene
        {
            get
            {
                return this.scene;
            }
            set
            {
                this.scene = value;
            }
        }
    }
}