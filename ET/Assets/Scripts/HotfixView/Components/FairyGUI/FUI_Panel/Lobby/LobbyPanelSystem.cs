using System;
using UnityEngine;

namespace ET.Client
{
	public static class LobbyPanelSystem
	{
		public static void Awake(this LobbyPanel self)
		{

		}
		
		public static void RegisterUIEvent(this LobbyPanel self)
		{
			self.FUILobbyPanel.TestABtn.AddListner(() =>
			{
				self.Root().GetComponent<FUIComponent>().HideAndShowPanelStackAsync(PanelId.LobbyPanel, PanelId.TestAPanel).NoContext();
			});
			
			self.FUILobbyPanel.EnterMap.AddListnerAsync(self.EnterMap);
		}

		public static void OnShow(this LobbyPanel self, Entity contextData = null)
		{

		}

		public static void OnHide(this LobbyPanel self)
		{

		}

		public static void BeforeUnload(this LobbyPanel self)
		{

		}

		private static async ETTask EnterMap(this LobbyPanel self)
		{
			await EnterMapHelper.EnterMapAsync(self.Root());
			self.Root().GetComponent<FUIComponent>().HidePanel(PanelId.LobbyPanel);
		}
	}
}