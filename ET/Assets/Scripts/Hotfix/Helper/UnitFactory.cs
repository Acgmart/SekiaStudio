using System;
using Unity.Mathematics;

namespace ET.Client
{
    public static partial class UnitFactory
    {
        public static Unit Create(Scene currentScene, UnitInfo unitInfo)
        {
	        UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
	        Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
	        unitComponent.Add(unit);
	        
	        unit.Position = unitInfo.Position;
	        unit.Forward = unitInfo.Forward;
	        
	        NumericComponent numericComponent = unit.AddComponent<NumericComponent>();

			foreach (var kv in unitInfo.KV)
			{
				numericComponent.Set(kv.Key, kv.Value);
			}
	        
	        unit.AddComponent<MoveComponent>();
	        if (unitInfo.MoveInfo != null)
	        {
		        if (unitInfo.MoveInfo.Points.Count > 0)
				{
					unitInfo.MoveInfo.Points[0] = unit.Position;
					unit.MoveToAsync(unitInfo.MoveInfo.Points).NoContext();
				}
	        }

	        unit.AddComponent<ObjectWait>();

	        unit.AddComponent<XunLuoPathComponent>();
	        
	        EventSystem.Instance.Publish(unit.Scene(), new AfterUnitCreate() {Unit = unit});
            return unit;
        }
    }
}

namespace ET.Server
{
	public static partial class UnitFactory
	{
		public static Unit Create(Scene scene, long id, int unitType, string mapName)
		{
			UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
			switch (unitType)
			{
				case UnitType.Player:
				{
					Unit unit = unitComponent.AddChildWithId<Unit, int>(id, 1001);
					unit.AddComponent<MoveComponent>();
					unit.Position = new float3(-10, 0, -10);
					unit.AddComponent<MailBoxComponent, int>(MailBoxType.OrderedMessage);

					NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
					numericComponent.Set(NumericType.Speed, 6f); // 速度是6米每秒
					numericComponent.Set(NumericType.AOI, 15000); // 视野15米
                    
					unitComponent.Add(unit);
					// 加入aoi
					unit.AddComponent<AOIEntity, int, float3>(9 * 1000, unit.Position);
					return unit;
				}
				default:
					throw new Exception($"not such unit type: {unitType}");
			}
		}
	}
}