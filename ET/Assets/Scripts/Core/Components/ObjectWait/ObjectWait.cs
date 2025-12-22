using System;
using System.Collections.Generic;
using System.Linq;

namespace ET
{
    [EntitySystemOf(typeof(ObjectWait))]
    public static partial class ObjectWaitSystem
    {
        [EntitySystem]
        private static void Awake(this ObjectWait self)
        {
            self.tcss.Clear();
        }
        
        [EntitySystem]
        private static void Destroy(this ObjectWait self)
        {
            foreach (object v in self.tcss.Values.ToArray())
            {
                ((IDestroyRun) v).SetResult();
            }
        }
        
        public static async ETTask<T> Wait<T>(this ObjectWait self) where T : struct, IWaitType
        {
            ResultCallback<T> tcs = new ResultCallback<T>();
            //http://www.pswp.cn/web/35495.shtml
            //获取当前异步方法的上下文
            //当前异步方法会执行ETAsyncTaskMethodBuilder.Create()方法创建ETTask和Bulder对象，以及编译器创建的状态机对象
            //  所有ETTask的TaskType默认都是Common 只在执行过SetContex后才变成WithContex
            //  需要上游执行WithContext明文指定Contex后才能执行GetContextAsync方法
            //  可以理解成async/await构成了task任务链，最外层WithContext赋值后传递给了链上的所有task
            //  SetResul后状态机MoveNext执行await后的代码
            //普通情况下只要在异步方法内调用await子异步 就会执行Bulder对象的AwaitUnsafeOnCompleted方法
            //  如果Bulder的ETTask对象带有上下文就会传递给子异步。反之则将子异步设置为Bulder的ETTask的上下文用于后续传递。
            //  最外层WithContext赋值时机如果是在await之前 那么父异步已经有了上下文可以直接传递给子异步
            //  最外层WithContext赋值时机如果在await之后，通过task链SetContex的while(true)循环传递到子异步
            //在SetContext方法中利用ContextTask获取父异步的Contex对象作为返回值
            //  如果Contex为null或者类型错误则不会出报错
            ETCancellationToken cancellationToken = await ETTaskHelper.GetContextAsync<ETCancellationToken>();
            self.tcss.Add(typeof (T), tcs);
            
            void CancelAction()
            {
                self.Notify(new T() { Error = WaitTypeError.Cancel });
            }
            
            T ret;
            try
            {
                cancellationToken?.Add(CancelAction);
                ret = await tcs.Task;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);    
            }
            return ret;
        }

        public static void Notify<T>(this ObjectWait self, T obj) where T : struct, IWaitType
        {
            Type type = typeof (T);
            if (!self.tcss.TryGetValue(type, out object tcs))
            {
                return;
            }

            self.tcss.Remove(type);
            ((ResultCallback<T>) tcs).SetResult(obj);
        }
    }
    
    public static class WaitTypeError
    {
        public const int Success = 0;
        public const int Destroy = 1;
        public const int Cancel = 2;
        public const int Timeout = 3;
    }
    
    public interface IWaitType
    {
        int Error
        {
            get;
            set;
        }
    }
    
    
    public interface IDestroyRun
    {
        void SetResult();
    }
    
    public class ResultCallback<K>: Object, IDestroyRun where K : struct, IWaitType
    {
        private ETTask<K> tcs;

        public ResultCallback()
        {
            this.tcs = ETTask<K>.Create(true);
        }

        public bool IsDisposed
        {
            get
            {
                return this.tcs == null;
            }
        }

        public ETTask<K> Task => this.tcs;

        public void SetResult(K k)
        {
            var t = tcs;
            this.tcs = null;
            t.SetResult(k);
        }

        public void SetResult()
        {
            var t = tcs;
            this.tcs = null;
            t.SetResult(new K() { Error = WaitTypeError.Destroy });
        }
    }

    //对ETCancellationToken的包装
    [ComponentOf]
    public class ObjectWait: Entity, IAwake, IDestroy
    {
        public Dictionary<Type, object> tcss = new Dictionary<Type, object>();
    }
}