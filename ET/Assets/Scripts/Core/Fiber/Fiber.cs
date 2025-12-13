using System;
using System.Collections.Generic;
using System.Threading;

namespace ET
{
    public static class FiberHelper
    {
        public static ActorId GetActorId(this Entity self)
        {
            Fiber root = self.Fiber();
            return new ActorId(root.FiberId, self.InstanceId);
        }
    }
    
    public class Fiber: IDisposable
    {
        // 该字段只能框架使用，绝对不能改成public，改了后果自负
        [StaticField]
        [ThreadStatic]
        public static Fiber Instance;
        
        public bool IsDisposed;
        
        public int FiberId;

        public Scene Root { get; }

        public EntitySystem EntitySystem { get; }
        public Mailboxes Mailboxes { get; private set; }
        public ThreadSynchronizationContext ThreadSynchronizationContext { get; }
        public ILog Log { get; }

        private readonly Queue<ETTask> frameFinishTasks = new();
        
        internal Fiber(int fiberId, int sceneType, string name)
        {
            this.FiberId = fiberId;
            this.EntitySystem = new EntitySystem();
            this.Mailboxes = new Mailboxes();
            this.ThreadSynchronizationContext = new ThreadSynchronizationContext();

            LogInvoker logInvoker = new()
                    { Fiber = this.FiberId, SceneName = SceneTypeSingleton.Instance.GetSceneName(sceneType) };
            this.Log = EventSystem.Instance.Invoke<LogInvoker, ILog>(logInvoker);
            
            this.Root = new Scene(this, fiberId, 1, sceneType, name);
        }

        internal void Update()
        {
            try
            {
                this.EntitySystem.Publish(new UpdateEvent());
            }
            catch (Exception e)
            {
                this.Log.Error(e);
            }
        }
        
        internal void LateUpdate()
        {
            try
            {
                this.EntitySystem.Publish(new LateUpdateEvent());
                FrameFinishUpdate();
                
                this.ThreadSynchronizationContext.Update();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public async ETTask WaitFrameFinish()
        {
            ETTask task = ETTask.Create(true);
            this.frameFinishTasks.Enqueue(task);
            await task;
        }

        private void FrameFinishUpdate()
        {
            while (this.frameFinishTasks.Count > 0)
            {
                ETTask task = this.frameFinishTasks.Dequeue();
                task.SetResult();
            }
        }

        public void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            this.IsDisposed = true;
            
            this.Root.Dispose();
        }
    }
}