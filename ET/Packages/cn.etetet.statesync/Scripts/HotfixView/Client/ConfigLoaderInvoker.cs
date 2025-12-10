using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ET
{
    [Invoke]
    public class GetAllConfigBytes: AInvokeHandler<ConfigLoader.GetAllConfigBytes, ETTask<Dictionary<Type, byte[]>>>
    {
        public override async ETTask<Dictionary<Type, byte[]>> Handle(ConfigLoader.GetAllConfigBytes args)
        {
            Dictionary<Type, byte[]> output = new Dictionary<Type, byte[]>();
            HashSet<Type> configTypes = CodeTypes.Instance.GetTypes(typeof (ConfigAttribute));

            List<string> startConfigs = new List<string>()
                {
                    "StartMachineConfigCategory",
                    "StartProcessConfigCategory",
                    "StartSceneConfigCategory",
                    "StartZoneConfigCategory",
                };

            foreach (Type configType in configTypes)
            {
                string configFilePath;
                if (startConfigs.Contains(configType.Name))
                {
                    configFilePath = $"Assets/Res/Config/Bytes/{Options.Instance.StartConfig}/{configType.Name}.bytes";
                }
                else
                {
                    configFilePath = $"Assets/Res/Config/Bytes/{configType.Name}.bytes";
                }
                TextAsset v = await ResourcesComponent.Instance.LoadAssetAsync<TextAsset>(configFilePath);
                output[configType] = v.bytes;
            }
            return output;
        }
    }
    
    [Invoke]
    public class GetOneConfigBytes: AInvokeHandler<ConfigLoader.GetOneConfigBytes, ETTask<byte[]>>
    {
        public override async ETTask<byte[]> Handle(ConfigLoader.GetOneConfigBytes args)
        {
            List<string> startConfigs = new List<string>()
            {
                "StartMachineConfigCategory", 
                "StartProcessConfigCategory", 
                "StartSceneConfigCategory", 
                "StartZoneConfigCategory",
            };

            string configName = args.ConfigName;
            string configFilePath;
            if (startConfigs.Contains(configName))
            {
                configFilePath = $"Assets/Res/Config/Bytes/{Options.Instance.StartConfig}/{configName}.bytes";    
            }
            else
            {
                configFilePath = $"Assets/Res/Config/Bytes/{configName}.bytes";
            }

            await ETTask.CompletedTask;
            return File.ReadAllBytes(configFilePath);
        }
    }
}