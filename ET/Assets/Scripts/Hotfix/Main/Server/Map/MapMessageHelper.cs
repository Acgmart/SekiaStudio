

using System.Collections.Generic;

namespace ET.Server
{
    public static partial class MapMessageHelper
    {
        public static void NoticeUnitAdd(Unit unit, Unit sendUnit)
        {
            M2C_CreateUnits createUnits = M2C_CreateUnits.Create();
            createUnits.Units.Add(UnitHelper.CreateUnitInfo(sendUnit));
            MapMessageHelper.SendToClient(unit, createUnits);
        }
        
        public static void NoticeUnitRemove(Unit unit, Unit sendUnit)
        {
            M2C_RemoveUnits removeUnits = M2C_RemoveUnits.Create();
            removeUnits.Units.Add(sendUnit.Id);
            MapMessageHelper.SendToClient(unit, removeUnits);
        }
        
        //将消息广播到客户端 需要有客户端Session和Unit的绑定关系
        public static void Broadcast(Unit unit, IMessage message)
        {
            (message as MessageObject).IsFromPool = false;
            Dictionary<long, EntityRef<AOIEntity>> dict = unit.GetBeSeePlayers();
            // 网络底层做了优化，同一个消息不会多次序列化
            
            foreach (AOIEntity u in dict.Values)
            {
                //Player和Unit的Id相同但是InstancID不同
                Scene root = unit.Root();
                Player aoiPlayer = root.GetComponent<PlayerComponent>().GetChild<Player>(u.Unit.Id);
                PlayerSessionComponent playerSessionComponent = aoiPlayer.GetComponent<PlayerSessionComponent>();
                MessageQueue.Instance.Send(root.Fiber.FiberId, new ActorId(root.Fiber.FiberId, playerSessionComponent.InstanceId), (MessageObject)message);
            }
        }
        
        //将消息发送到客户端 需要有客户端Session和Unit的绑定关系
        public static void SendToClient(Unit unit, IMessage message)
        {
            //Player和Unit的Id相同但是InstancID不同
            Scene root = unit.Root();
            Player player = root.GetComponent<PlayerComponent>().GetChild<Player>(unit.Id);
            PlayerSessionComponent playerSessionComponent = player.GetComponent<PlayerSessionComponent>();
            MessageQueue.Instance.Send(root.Fiber.FiberId, new ActorId(root.Fiber.FiberId, playerSessionComponent.InstanceId), (MessageObject)message);
        }
    }
}