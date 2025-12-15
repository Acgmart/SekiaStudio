using System;
using System.Collections.Generic;
using System.Net;

namespace ET
{
    public class Options : Singleton<Options>, ISingletonAwake
    {
        public string SceneName = "Server";
        public string StartConfig = "StartConfig/Localhost";
        public int Process = 1;
        public int Develop = 0;
        public int LogLevel = 0;
        public int Console = 0;

        //假定用户已知服务端外网IP和端口
        public string Port = "10101";

        public void Awake()
        {
        }

        private IPEndPoint innerIPPort;

        public IPEndPoint InnerIPPort
        {
            get
            {
                if (innerIPPort == null)
                {
                    this.innerIPPort = NetworkHelper.ToIPEndPoint($"127.0.0.1:{this.Port}");
                }

                return this.innerIPPort;
            }
        }
    }
}