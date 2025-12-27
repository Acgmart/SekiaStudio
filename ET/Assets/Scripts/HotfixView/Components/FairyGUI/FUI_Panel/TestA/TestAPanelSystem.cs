using System;
using FairyGUI;
using UnityEngine;

namespace ET.Client
{
	[FriendOf(typeof(TestAPanel))]
	public static class TestAPanelSystem
	{
		public static void Awake(this TestAPanel self)
		{

		}

		public static void RegisterUIEvent(this TestAPanel self)
		{
			var fuiCom = self.Root().GetComponent<FUIComponent>();
			self.FUITestAPanel.OpenTestBBtn.AddListner(() =>
			{
				var context = fuiCom.AddChild<TestBPanel_ContextData>();
				context.Data = "TestBPanel_ContextData";
				fuiCom.HideAndShowPanelStackAsync(PanelId.TestAPanel, PanelId.TestBPanel, context).NoContext();
			});
			
			self.FUITestAPanel.HideBtn.AddListner(() =>
			{
				fuiCom.HidePanel(PanelId.TestAPanel);
			});
			
			self.FUITestAPanel.LanguageCombo.items = new string[] {"简体中文", "繁體中文", "English"};
			self.FUITestAPanel.LanguageCombo.selectedIndex = 0;
			self.FUITestAPanel.LanguageCombo.onChanged.Add(() =>
			{
				switch (self.FUITestAPanel.LanguageCombo.selectedIndex)
				{
					case 0:
						PrintUnitsName();
						break;
					
					case 1:
						PrintUnitsName();
						break;
					
					case 2:
						PrintUnitsName();
						break;
				}
			});
		}
		
		private static void PrintUnitsName()
		{
			var dataMap = UnitConfigCategory.Instance.GetAll();
			foreach (var kv in dataMap)
			{
				Log.Info($"id: {kv.Key}, name: {kv.Value.Name}");
			}
		}

		public static void OnShow(this TestAPanel self, Entity contexData = null)
		{

		}

		public static void OnHide(this TestAPanel self)
		{

		}

		public static void BeforeUnload(this TestAPanel self)
		{

		}
	}
}
