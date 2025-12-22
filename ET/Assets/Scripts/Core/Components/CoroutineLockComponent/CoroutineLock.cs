using System;

namespace ET
{
    [EntitySystemOf(typeof(CoroutineLock))]
    public static partial class CoroutineLockSystem
    {
        [EntitySystem]
        private static void Awake(this CoroutineLock self, long type, long k, int count)
        {
            self.type = type;
            self.key = k;
            self.level = count;
        }
        
        [EntitySystem]
        private static void Destroy(this CoroutineLock self)
        {
            //完成一个任务时开始下一个任务
            //如果不存在下一个任务就会移除key对应的task队列重置
            self.GetParent<CoroutineLockQueue>()?
                .GetParent<CoroutineLockQueueType>()?
                .GetParent<CoroutineLockComponent>()?.RunNextCoroutine(self.type, self.key, self.level + 1);
            self.type = 0;
            self.key = 0;
            self.level = 0;
        }
    }
    
    [ChildOf(typeof(CoroutineLockQueue))]
    public class CoroutineLock: Entity, IAwake<long, long, int>, IDestroy
    {
        public long type;
        public long key;
        public int level;
    }
}