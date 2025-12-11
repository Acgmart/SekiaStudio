namespace ET.Server
{
    [EntitySystemOf(typeof(SessionPlayerComponent))]
    public static partial class SessionPlayerComponentSystem
    {
        [EntitySystem]
        private static void Destroy(this SessionPlayerComponent self)
        {
            Scene root = self.Root();
            if (root.IsDisposed)
            {
                return;
            }

            //处理Unit下线逻辑
        }
        
        [EntitySystem]
        private static void Awake(this SessionPlayerComponent self)
        {

        }
    }
}