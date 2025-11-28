using System;
using UnityEngine;
using System.Reflection;

namespace ET
{
    public class Init: MonoBehaviour
    {
        private void Start()
        {
            this.StartAsync().NoContext();
        }
		
        private async ETTask StartAsync()
        {
            DontDestroyOnLoad(gameObject);
			
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Error(e.ExceptionObject.ToString());
            };

            World.Instance.AddSingleton<Options>();
            GlobalConfig globalConfig = Resources.Load<GlobalConfig>("GlobalConfig");
            Options.Instance.SceneName = globalConfig.SceneName;
			
            World.Instance.AddSingleton<Logger>().Log = new UnityLogger();
            ETTask.ExceptionHandler += Log.Error;
			
            World.Instance.AddSingleton<TimeInfo>();
            World.Instance.AddSingleton<FiberManager>();

            await World.Instance.AddSingleton<ResourcesComponent>().CreatePackageAsync("DefaultPackage", true);
            
#if INITED
            World.Instance.AddSingleton<CodeTypes, Assembly[]>(new[]
            {
                typeof (World).Assembly, //Core
                typeof (Define).Assembly, //Loader
                typeof (FiberInit_Main).Assembly, //Model
                typeof (Client.ResourcesLoaderComponent).Assembly, //ModelView
                typeof (ETCancellationTokenHelper).Assembly, //Hotfix
                typeof (Client.ResourcesLoaderComponentSystem).Assembly, //HotfixView
            });

            
            WinPeriod.Init();

            // 注册Mongo type
            MongoRegister.Init();
            
            MemoryPackRegister.Init();
            
            // 注册Entity序列化器
            EntitySerializeRegister.Init();

            World.Instance.AddSingleton<SceneTypeSingleton, Type>(typeof(SceneType));
            World.Instance.AddSingleton<ObjectPool>();
            World.Instance.AddSingleton<IdGenerater>();
            World.Instance.AddSingleton<OpcodeType>();
            
            World.Instance.AddSingleton<MessageQueue>();
            World.Instance.AddSingleton<NetServices>();
            
            LogMsg logMsg = World.Instance.AddSingleton<LogMsg>();
            logMsg.AddIgnore(LoginOuter.C2G_Ping);
            logMsg.AddIgnore(LoginOuter.G2C_Ping);
            
            World.Instance.AddSingleton<EntitySystemSingleton>();
            World.Instance.AddSingleton<EventSystem>();
            World.Instance.AddSingleton<AIDispatcherComponent>();
            World.Instance.AddSingleton<MessageDispatcher>();
            World.Instance.AddSingleton<MessageSessionDispatcher>();
            World.Instance.AddSingleton<NumericWatcherComponent>();
            World.Instance.AddSingleton<Server.HttpDispatcher>();
            World.Instance.AddSingleton<Client.UIEventComponent>();
            
            await World.Instance.AddSingleton<ConfigLoader>().LoadAsync();
            
            await FiberManager.Instance.Create(SchedulerType.Main, SceneType.Main, 0, SceneType.Main, "");
#endif
        }

        private void Update()
        {
            TimeInfo.Instance.Update();
            FiberManager.Instance.Update();
        }

        private void LateUpdate()
        {
            FiberManager.Instance.LateUpdate();
        }

        private void OnApplicationQuit()
        {
            World.Instance.Dispose();
        }
    }
}