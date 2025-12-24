using System.Collections.Generic;
using System.Text;

namespace ET.Server
{
    [EntitySystemOf(typeof(Cell))]
    public static partial class CellSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.Cell self)
        {

        }
        
        [EntitySystem]
        private static void Destroy(this Cell self)
        {
            self.AOIUnits.Clear();

            self.SubsEnterEntities.Clear();

            self.SubsLeaveEntities.Clear();
        }

        public static void Add(this Cell self, AOIEntity aoiEntity)
        {
            self.AOIUnits.Add(aoiEntity.Id, aoiEntity);
        }

        public static void Remove(this Cell self, AOIEntity aoiEntity)
        {
            self.AOIUnits.Remove(aoiEntity.Id);
        }

        public static string CellIdToString(this long cellId)
        {
            int y = (int)(cellId & 0xffffffff);
            int x = (int)((ulong)cellId >> 32);
            return $"{x}:{y}";
        }

        public static string CellIdToString(this HashSet<long> cellIds)
        {
            StringBuilder sb = new StringBuilder();
            foreach (long cellId in cellIds)
            {
                sb.Append(cellId.CellIdToString());
                sb.Append(",");
            }

            return sb.ToString();
        }

    }
    
    [ChildOf(typeof(AOIManagerComponent))]
    public class Cell: Entity, IAwake, IDestroy
    {
        // 处在这个cell的单位
        public Dictionary<long, EntityRef<AOIEntity>> AOIUnits = new Dictionary<long, EntityRef<AOIEntity>>();

        // 订阅了这个Cell的进入事件
        public Dictionary<long, EntityRef<AOIEntity>> SubsEnterEntities = new Dictionary<long, EntityRef<AOIEntity>>();

        // 订阅了这个Cell的退出事件
        public Dictionary<long, EntityRef<AOIEntity>> SubsLeaveEntities = new Dictionary<long, EntityRef<AOIEntity>>();
    }
}