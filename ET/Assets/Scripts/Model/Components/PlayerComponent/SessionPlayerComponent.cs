namespace ET.Server
{
	[EntitySystemOf(typeof(SessionPlayerComponent))]
	public static partial class SessionPlayerComponentSystem
	{
		[EntitySystem]
		private static void Destroy(this SessionPlayerComponent self)
		{
			Scene root = self.Root();
			if (root.IsDisposed)
			{
				return;
			}

			//处理Unit下线逻辑
		}
        
		[EntitySystem]
		private static void Awake(this SessionPlayerComponent self)
		{

		}
	}
	
	//挂在服务端Session上 用于绑定Session与Player
	[ComponentOf(typeof(Session))]
	public class SessionPlayerComponent : Entity, IAwake, IDestroy
	{
		private EntityRef<Player> player;

		public Player Player
		{
			get
			{
				return this.player;
			}
			set
			{
				this.player = value;
			}
		}
	}
}