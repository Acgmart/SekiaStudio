using System;
using System.Collections.Generic;
using System.IO;

namespace ET
{
	public static class FileHelper
	{
		public static List<string> GetAllFiles(string dir, string searchPattern = "*")
		{
			List<string> list = new List<string>();
			GetAllFiles(list, dir, searchPattern);
			return list;
		}
		
		public static void GetAllFiles(List<string> files, string dir, string searchPattern = "*")
		{
			string[] fls = Directory.GetFiles(dir, searchPattern);
			foreach (string fl in fls)
			{
				files.Add(fl);
			}

			string[] subDirs = Directory.GetDirectories(dir);
			foreach (string subDir in subDirs)
			{
				GetAllFiles(files, subDir, searchPattern);
			}
		}
	}
}
