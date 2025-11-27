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
                typeof (Entry).Assembly, //Model
                typeof (Client.ResourcesLoaderComponent).Assembly, //ModelView
                typeof (ETCancellationTokenHelper).Assembly, //Hotfix
                typeof (Client.ResourcesLoaderComponentSystem).Assembly, //HotfixView
            });

            ET.Entry.Start();
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