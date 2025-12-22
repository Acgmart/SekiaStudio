namespace ET
{
    [UniqueId]
    public static partial class ErrorCode
    {
        public const int ERR_Success = 0;

        // 1-11004 是SocketError请看SocketError定义
        //-----------------------------------
        // 100000-100000000是Core层的错误
        
        // 这里配置逻辑层的错误码
        // 100000000以上是逻辑层的错误
        private const int ERR_WithException = 100000000;
        public const int ERR_SessionSendOrRecvTimeout = ERR_WithException + PackageType.Core * 1000 + 1;
        public const int ERR_NotFoundActor = ERR_WithException + PackageType.ActorLocation * 1000 + 1;
        public const int ERR_RpcFail = ERR_WithException + PackageType.ActorLocation * 1000 + 2;
        public const int ERR_MessageTimeout = ERR_WithException + PackageType.ActorLocation * 1000 + 3;
        public const int ERR_ActorLocationSenderTimeout2 = ERR_WithException + PackageType.ActorLocation * 1000 + 4;
        public const int ERR_ActorLocationSenderTimeout3 = ERR_WithException + PackageType.ActorLocation * 1000 + 5;
        public const int ERR_ActorLocationSenderTimeout4 = ERR_WithException + PackageType.ActorLocation * 1000 + 6;
        
        // 200000000以上不抛异常
        private const int ERR_WithoutException = 200000000;
        public const int ERR_Cancel = ERR_WithoutException + PackageType.Core * 1000 + 1;
        public const int ERR_Timeout = ERR_WithoutException + PackageType.Core * 1000 + 2;
        
        public static bool IsRpcNeedThrowException(int error)
        {
            if (error == 0)
            {
                return false;
            }
            if (error > ERR_WithoutException)
            {
                return false;
            }

            return true;
        }
    }
}