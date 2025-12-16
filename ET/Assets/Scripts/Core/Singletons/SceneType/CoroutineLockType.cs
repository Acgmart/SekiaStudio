namespace ET
{
    public static class CoroutineLockType
    {
        public const int Location = PackageType.ActorLocation * 1000 + 1;                  // location进程上使用
        public const int MessageLocationSender = PackageType.ActorLocation * 1000 + 2;       // MessageLocationSender中队列消息 
        public const int Mailbox = PackageType.Core * 1000 + 1;                   // Mailbox中队列
        public const int ResourcesLoader = PackageType.YooAssets * 1000 + 2;
    }
}