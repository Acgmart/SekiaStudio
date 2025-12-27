using System.Collections.Generic;
using FairyGUI.Dynamic;
using FairyGUI;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(FUIAssetComponent))]
    public static partial class FUIAssetComponentSystem
    {
        [EntitySystem]
        private static void Awake(this FUIAssetComponent self,  bool unloadUnusedUIPackageImmediately)
        {
            self.UnloadUnusedUIPackageImmediately = unloadUnusedUIPackageImmediately;
            
            void LoadUIPackageAsyncHandler(string packageName, LoadUIPackageBytesCallback callback)
            {
                self.LoadUIPackageAsyncInner(packageName, callback).NoContext();
            }

            void LoadTextureAsyncHandler(string packageName, string assetName, string extension, LoadTextureCallback callback)
            {
                self.LoadTextureAsyncInner(assetName, callback).NoContext();
            }

            void LoadAudioClipAsyncHandler(string packageName, string assetName, string extension, LoadAudioClipCallback callback)
            {
                self.LoadAudioClipAsyncInner(assetName, callback).NoContext();
            }

            self.Locations = new Dictionary<int, string>();
            var assetLoader = new DelegateUIAssetLoader();
            assetLoader.LoadUIPackageBytesAsyncHandlerImpl = LoadUIPackageAsyncHandler;
            assetLoader.LoadUIPackageBytesHandlerImpl = self.LoadUIPackageSyncInner;
            assetLoader.LoadTextureAsyncHandlerImpl = LoadTextureAsyncHandler;
            assetLoader.UnloadTextureHandlerImpl = self.UnloadAssetInner;
            assetLoader.LoadAudioClipAsyncHandlerImpl = LoadAudioClipAsyncHandler;
            assetLoader.UnloadAudioClipHandlerImpl = self.UnloadAssetInner;

            self.AssetLoader = assetLoader;

            ResourcesLoaderComponent rlc = self.Root().GetComponent<ResourcesLoaderComponent>();
            string file = "Assets/Bundles/FUI/UIPackageMapping.bytes";
            byte[] mappingData = rlc.LoadBytesSync(file);
            self.PackageHelper = new UIPackageMapping(mappingData);

            self.UIAssetManager = new UIAssetManager();
            self.UIAssetManager.Initialize(self);
        }
        
        [EntitySystem]
        private static void Destroy(this FUIAssetComponent self)
        {
            self.UIAssetManager.Dispose();
            self.UIAssetManager = null;
            self.AssetLoader = null;

            ResourcesLoaderComponent rlc = self.Root().GetComponent<ResourcesLoaderComponent>();
            if (rlc != null && !rlc.IsDisposed)
            {
                foreach (string location in self.Locations.Values)
                {
                    rlc.UnloadAsset(location);
                }
            }

            self.Locations.Clear();
        }
        
        public static ETTask<GObject> CreateObjectFromURLAsync(this FUIAssetComponent self, string url)
        {
            ETTask<GObject> task = ETTask<GObject>.Create(true);
            UIPackage.CreateObjectFromURLAsync(url, result =>
            {
                task.SetResult(result);
            });
            return task;
        }

        public static ETTask<GObject> CreateObjectAsync(this FUIAssetComponent self, string pkgName, string resName)
        {
            ETTask<GObject> task = ETTask<GObject>.Create(true);
            UIPackage.CreateObjectAsync(pkgName, resName, result =>
            {
                task.SetResult(result);
            });
            return task;
        }

        public static GObject CreateObject(this FUIAssetComponent self, string pkgName, string resName)
        {
            return UIPackage.CreateObject(pkgName, resName);
        }
        
        public static void UnloadUnusedUIPackages(this FUIAssetComponent self)
        {
            UIPackage.RemoveUnusedPackages();
        }

        private static void LoadUIPackageSyncInner(this FUIAssetComponent self, string packageName, out byte[] bytes, out string assetNamePrefix)
        {
            string location = "{0}{1}".Fmt(packageName, "_fui");
            
            ResourcesLoaderComponent rlc = self.Root().GetComponent<ResourcesLoaderComponent>();
            byte[] descData = rlc.LoadBytesSync(location);
            rlc.UnloadAsset(location);
            
            bytes = descData;
            assetNamePrefix = packageName;
        }

        private static async ETTask LoadUIPackageAsyncInner(this FUIAssetComponent self, string packageName, LoadUIPackageBytesCallback callback)
        {
            string location = "{0}{1}".Fmt(packageName, "_fui");
            ResourcesLoaderComponent rlc = self.Root().GetComponent<ResourcesLoaderComponent>();
            byte[] descData = await rlc.LoadBytesAsync(location);
            rlc.UnloadAsset(location);
                
            callback(descData, packageName);
        }

        private static async ETTask LoadTextureAsyncInner(this FUIAssetComponent self, string assetName, LoadTextureCallback callback)
        {
            ResourcesLoaderComponent rlc = self.Root().GetComponent<ResourcesLoaderComponent>();
            Texture res = await rlc.LoadAssetAsync<Texture>(assetName);

            if (res != null)
                self.Locations[res.GetInstanceID()] = assetName;

            callback(res);
        }

        private static async ETTask LoadAudioClipAsyncInner(this FUIAssetComponent self, string assetName, LoadAudioClipCallback callback)
        {
            ResourcesLoaderComponent rlc = self.Root().GetComponent<ResourcesLoaderComponent>();
            AudioClip res = await rlc.LoadAssetAsync<AudioClip>(assetName);

            if (res != null)
                self.Locations[res.GetInstanceID()] = assetName;

            callback(res);
        }

        private static void UnloadAssetInner(this FUIAssetComponent self, UnityEngine.Object obj)
        {
            if (obj == null)
                return;

            int instanceId = obj.GetInstanceID();
            if (!self.Locations.TryGetValue(instanceId, out string location))
                return;

            self.Locations.Remove(instanceId);

            ResourcesLoaderComponent rlc = self.Root().GetComponent<ResourcesLoaderComponent>();
            if (rlc != null && !rlc.IsDisposed)
            {
                rlc.UnloadAsset(location);
            }
        }
    }
    
    [ComponentOf(typeof(Scene))]
    public class FUIAssetComponent : Entity, IAwake<bool>, IDestroy, IUIAssetManagerConfiguration
    {
        public UIAssetManager UIAssetManager;

        public Dictionary<int, string> Locations;
        
        public IUIPackageHelper PackageHelper { get; set; }
        
        public IUIAssetLoader AssetLoader { get;  set;}
        
        public bool UnloadUnusedUIPackageImmediately { get; set; }
    }
}