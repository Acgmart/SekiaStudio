namespace ET
{
    public static class EntityHelper
    {
        //节点树上最近的Scene节点
        public static Scene Scene(this Entity entity)
        {
            return entity.IScene;
        }

        //节点树上最远的Scene节点
        public static Scene Root(this Entity entity)
        {
            return entity.IScene.Fiber.Root;
        }

        public static Fiber Fiber(this Entity entity)
        {
            return entity.IScene.Fiber;
        }
    }
}