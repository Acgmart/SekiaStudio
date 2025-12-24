using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ET
{
    [Invoke]
    public class GetAllConfigBytes : AInvokeHandler<ConfigLoader.GetAllConfigBytes, ETTask<Dictionary<Type, byte[]>>>
    {
        public override async ETTask<Dictionary<Type, byte[]>> Handle(ConfigLoader.GetAllConfigBytes args)
        {
            Dictionary<Type, byte[]> output = new Dictionary<Type, byte[]>();
            HashSet<Type> configTypes = CodeTypes.Instance.GetTypes(typeof(ConfigAttribute));

            foreach (Type configType in configTypes)
            {
                string configFilePath = $"Assets/Res/Config/Bytes/{configType.Name}.bytes";
                TextAsset v = await ResourcesComponent.Instance.LoadAssetAsync<TextAsset>(configFilePath);
                output[configType] = v.bytes;
            }
            return output;
        }
    }

    [Invoke]
    public class GetOneConfigBytes : AInvokeHandler<ConfigLoader.GetOneConfigBytes, ETTask<byte[]>>
    {
        public override async ETTask<byte[]> Handle(ConfigLoader.GetOneConfigBytes args)
        {
            string configName = args.ConfigName;
            string configFilePath = $"Assets/Res/Config/Bytes/{configName}.bytes";
            TextAsset v = await ResourcesComponent.Instance.LoadAssetAsync<TextAsset>(configFilePath);
            return v.bytes;
        }
    }
}