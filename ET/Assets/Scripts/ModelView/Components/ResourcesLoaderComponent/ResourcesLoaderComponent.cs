using System.Collections.Generic;
using UnityEngine.SceneManagement;
using YooAsset;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(ResourcesLoaderComponent))]
    public static partial class ResourcesLoaderComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ResourcesLoaderComponent self)
        {
            self.package = YooAssets.GetPackage("DefaultPackage");
        }

        [EntitySystem]
        private static void Awake(this ResourcesLoaderComponent self, string packageName)
        {
            self.package = YooAssets.GetPackage(packageName);
        }

        [EntitySystem]
        private static void Destroy(this ResourcesLoaderComponent self)
        {
            foreach (var kv in self.handlers)
            {
                switch (kv.Value)
                {
                    case AssetHandle handle:
                        handle.Release();
                        break;
                    case AllAssetsHandle handle:
                        handle.Release();
                        break;
                    case SubAssetsHandle handle:
                        handle.Release();
                        break;
                    case RawFileHandle handle:
                        handle.Release();
                        break;
                    case YooAsset.SceneHandle handle:
                        handle.UnloadAsync();
                        break;
                }
            }
        }
        
        //异步加载
        public static async ETTask<T> LoadAssetAsync<T>(this ResourcesLoaderComponent self, string location) where T : UnityEngine.Object
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAssetAsync<T>(location);

                await handler.Task;

                self.handlers.Add(location, handler);
            }

            return (T)((AssetHandle)handler).AssetObject;
        }
        
        //同步加载
        public static T LoadAssetSync<T>(this ResourcesLoaderComponent self, string location) where T : UnityEngine.Object
        {
            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAssetSync<T>(location);
                self.handlers.Add(location, handler);
            }

            return (T)((AssetHandle)handler).AssetObject;
        }

        //异步加载
        public static async ETTask<byte[]> LoadBytesAsync(this ResourcesLoaderComponent self, string location)
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAssetAsync<TextAsset>(location);

                await handler.Task;

                self.handlers.Add(location, handler);
            }

            return ((TextAsset)((AssetHandle)handler).AssetObject).bytes;
        }
        
        //同步加载
        public static byte[] LoadBytesSync(this ResourcesLoaderComponent self, string location)
        {
            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAssetSync<TextAsset>(location);
                self.handlers.Add(location, handler);
            }

            return ((TextAsset)((AssetHandle)handler).AssetObject).bytes;
        }

        public static async ETTask<Dictionary<string, T>> LoadAllAssetsAsync<T>(this ResourcesLoaderComponent self, string location) where T : UnityEngine.Object
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (!self.handlers.TryGetValue(location, out handler))
            {
                handler = self.package.LoadAllAssetsAsync<T>(location);
                await handler.Task;
                self.handlers.Add(location, handler);
            }

            Dictionary<string, T> dictionary = new Dictionary<string, T>();
            foreach (UnityEngine.Object assetObj in ((AllAssetsHandle)handler).AllAssetObjects)
            {
                T t = assetObj as T;
                dictionary.Add(t.name, t);
            }

            return dictionary;
        }

        public static async ETTask LoadSceneAsync(this ResourcesLoaderComponent self, string location, LoadSceneMode loadSceneMode)
        {
            using CoroutineLock coroutineLock = await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.ResourcesLoader, location.GetHashCode());

            HandleBase handler;
            if (self.handlers.TryGetValue(location, out handler))
            {
                return;
            }

            handler = self.package.LoadSceneAsync(location);

            await handler.Task;
            self.handlers.Add(location, handler);
        }
        
        public static void UnloadAsset(this ResourcesLoaderComponent self, string location)
        {
            if (self.handlers.TryGetValue(location, out HandleBase handler))
            {
                handler.Release();
                self.handlers.Remove(location);
            }
            else
            {
                Log.Error($"资源{location}不存在");
            }
        }
    }
    
    /// <summary>
    /// 用来管理资源，生命周期跟随Parent，比如CurrentScene用到的资源应该用CurrentScene的ResourcesLoaderComponent来加载
    /// 这样CurrentScene释放后，它用到的所有资源都释放了
    /// </summary>
    [ComponentOf]
    public class ResourcesLoaderComponent : Entity, IAwake, IAwake<string>, IDestroy
    {
        public ResourcePackage package;
        public Dictionary<string, HandleBase> handlers = new();
    }
}