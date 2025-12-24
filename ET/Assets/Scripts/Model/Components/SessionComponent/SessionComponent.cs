namespace ET.Client
{
	//客户端用于快速访问本地Session
	[ComponentOf(typeof(Scene))]
	public class SessionComponent: Entity, IAwake, IDestroy
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
