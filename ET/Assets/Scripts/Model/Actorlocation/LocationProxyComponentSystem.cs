using System;
using System.Collections.Generic;

namespace ET.Server
{
    public static partial class LocationProxyComponentSystem
    {
        public static async ETTask Add(this LocationManagerComoponent self, int type, long key, ActorId actorId)
        {
            Fiber fiber = self.Fiber();
            Log.Info($"location proxy add {key}, {actorId} {TimeInfo.Instance.ServerNow()}");
            await self.Get(type).Add(key, actorId);
        }

        public static async ETTask Remove(this LocationManagerComoponent self, int type, long key)
        {
            Fiber fiber = self.Fiber();
            Log.Info($"location proxy remove {key}, {TimeInfo.Instance.ServerNow()}");
            await self.Get(type).Remove(key);
        }

        public static async ETTask<ActorId> Get(this LocationManagerComoponent self, int type, long key)
        {
            if (key == 0)
            {
                throw new Exception($"get location key 0");
            }
            return await self.Get(type).Get(key);
        }

        public static async ETTask AddLocation(this Entity self, int type)
        {
            await self.Root().GetComponent<LocationManagerComoponent>().Add(type, self.Id, self.GetActorId());
        }

        public static async ETTask RemoveLocation(this Entity self, int type)
        {
            await self.Root().GetComponent<LocationManagerComoponent>().Remove(type, self.Id);
        }
    }
}