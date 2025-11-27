using System;
using System.Collections.Generic;

namespace ET
{
    public class Options: Singleton<Options>, ISingletonAwake
    {
        public string SceneName = "Server";
        public string StartConfig = "StartConfig/Localhost";
        public int Process = 1;
        public int Develop = 0;
        public int LogLevel = 0;
        public int Console = 0;

        public void Awake()
        {
        }
    }
}