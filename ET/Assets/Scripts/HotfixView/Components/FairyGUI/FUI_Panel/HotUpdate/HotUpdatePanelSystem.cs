using System;
using UnityEngine;

namespace ET.Client
{
	public static class HotUpdatePanelSystem
	{
		public static void Awake(this HotUpdatePanel self)
		{

		}

		public static void RegisterUIEvent(this HotUpdatePanel self)
		{

		}

		public static void OnShow(this HotUpdatePanel self, Entity contexData = null)
		{

		}

		public static void OnHide(this HotUpdatePanel self)
		{

		}

		public static void BeforeUnload(this HotUpdatePanel self)
		{

		}

		public static void OnPatchDownloadProgress(this HotUpdatePanel self, int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
		{
			self.FUIHotUpdatePanel.ProgressBar.value = 100.0f * currentDownloadBytes / totalDownloadBytes;
		}
	}
}