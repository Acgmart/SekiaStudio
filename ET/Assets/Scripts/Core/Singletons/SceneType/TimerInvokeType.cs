namespace ET
{
    public static class TimerInvokeType
    {
        public const int MessageLocationSenderChecker = PackageType.ActorLocation * 1000 + 2;
        public const int AITimer = PackageType.AI * 1000 + 1;
        public const int MoveTimer = PackageType.Move * 1000 + 1;
    }
}