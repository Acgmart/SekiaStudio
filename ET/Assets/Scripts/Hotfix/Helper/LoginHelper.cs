using System.Net;

namespace ET.Client
{
    public static class LoginHelper
    {
        public static async ETTask Login(Scene root, string address, string account, string password)
        {
            long playerId = await LoginAsync(root, address, account, password);

            root.GetComponent<MyPlayerComponent>().MyId = playerId;
            
            await EventSystem.Instance.PublishAsync(root, new LoginFinish());
        }
        
        
        public static async ETTask<long> LoginAsync(Scene root, string address, string account, string password)
        {
            IPEndPoint gateAddress = NetworkHelper.ToIPEndPoint(address);

#if UNITY_WEBGL
            root.AddComponent<NetComponent, IKcpTransport>(new WebSocketTransport(gateAddress.Address.AddressFamily));
#else
            root.AddComponent<NetComponent, IKcpTransport>(new UdpTransport(gateAddress.Address.AddressFamily));
#endif

            NetComponent netComponent = root.GetComponent<NetComponent>();
            
            // 创建一个gate Session,并且保存到SessionComponent中
            Session gateSession = netComponent.Create(gateAddress);
            gateSession.AddComponent<PingComponent>();
            root.AddComponent<SessionComponent>().Session = gateSession;
            C2G_LoginGate c2GLoginGate = C2G_LoginGate.Create();
            G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await gateSession.Call(c2GLoginGate);
            Log.Debug("登陆gate成功!");
            
            return g2CLoginGate.PlayerId;
        }
    }
}