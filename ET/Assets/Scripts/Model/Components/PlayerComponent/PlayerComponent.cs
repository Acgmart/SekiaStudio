using System.Collections.Generic;
using System.Linq;

namespace ET.Server
{
	[FriendOf(typeof(PlayerComponent))]
	public static partial class PlayerComponentSystem
	{
		public static void Add(this PlayerComponent self, Player player)
		{
			self.dictionary.Add(player.Account, player);
		}
        
		public static void Remove(this PlayerComponent self, Player player)
		{
			self.dictionary.Remove(player.Account);
			player.Dispose();
		}
        
		public static Player GetByAccount(this PlayerComponent self,  string account)
		{
			self.dictionary.TryGetValue(account, out EntityRef<Player> player);
			return player;
		}
	}
	
	//服务端管理所有在线Player的组件
	[ComponentOf(typeof(Scene))]
	public class PlayerComponent : Entity, IAwake, IDestroy
	{
		public Dictionary<string, EntityRef<Player>> dictionary = new Dictionary<string, EntityRef<Player>>();
	}
}