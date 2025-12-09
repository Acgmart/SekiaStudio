namespace ET.Server
{
	[MessageSessionHandler(SceneType.Gate)]
	public class C2G_EnterMapHandler : MessageSessionHandler<C2G_EnterMap, G2C_EnterMap>
	{
		protected override async ETTask Run(Session session, C2G_EnterMap request, G2C_EnterMap response)
		{
            await ETTask.CompletedTask;
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
			Scene scene = player.Root();
			Unit unit = UnitFactory.Create(scene, player.Id, UnitType.Player);
			response.MyId = player.Id;
            await unit.AddLocation(LocationType.Unit);

            // 等到一帧的最后面再传送，先让G2C_EnterMap返回，否则传送消息可能比G2C_EnterMap还早
            TransferAtFrameFinish(unit, session).NoContext();
		}

        public static async ETTask TransferAtFrameFinish(Unit unit, Session session)
        {
            await unit.Fiber().WaitFrameFinish();

            Scene root = unit.Root();

            // 通知客户端开始切场景
            M2C_StartSceneChange m2CStartSceneChange = M2C_StartSceneChange.Create();
            m2CStartSceneChange.SceneInstanceId = root.InstanceId;
            m2CStartSceneChange.SceneName = "Map2";
            session.Send(m2CStartSceneChange);

            // 通知客户端创建My Unit
            M2C_CreateMyUnit m2CCreateUnits = M2C_CreateMyUnit.Create();
            m2CCreateUnits.Unit = UnitHelper.CreateUnitInfo(unit);
            session.Send(m2CCreateUnits);
        }
    }
}