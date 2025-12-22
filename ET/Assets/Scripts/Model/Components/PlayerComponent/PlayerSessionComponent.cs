namespace ET.Server
{
	//挂在Player上 链接对应玩家的Session 用于转发消息到玩家客户端
	[ComponentOf(typeof(Player))]
	public class PlayerSessionComponent : Entity, IAwake
	{
		private EntityRef<Session> session;

		public Session Session
		{
			get
			{
				return this.session;
			}
			set
			{
				this.session = value;
			}
		}
	}
}