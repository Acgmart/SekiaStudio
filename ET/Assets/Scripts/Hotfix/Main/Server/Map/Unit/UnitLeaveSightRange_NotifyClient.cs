namespace ET.Server
{
    // 离开视野
    [Publish(SceneType.Gate)]
    public class UnitLeaveSightRange_NotifyClient : APublishHandler<UnitLeaveSightRange>
    {
        protected override async ETTask Run(Scene scene, UnitLeaveSightRange args)
        {
            await ETTask.CompletedTask;
            AOIEntity a = args.A;
            AOIEntity b = args.B;
            if (a.Unit.Type() != UnitType.Player)
            {
                return;
            }

            MapMessageHelper.NoticeUnitRemove(a.GetParent<Unit>(), b.GetParent<Unit>());
        }
    }
}