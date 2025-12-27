namespace ET
{
    public static class CoroutineLockType
    {
        public const int Mailbox = PackageType.Core * 1000 + 1;                   // Mailbox中队列
        public const int ResourcesLoader = PackageType.YooAssets * 1000 + 2;
        public const int LoadingPanels = 8;
    }
}