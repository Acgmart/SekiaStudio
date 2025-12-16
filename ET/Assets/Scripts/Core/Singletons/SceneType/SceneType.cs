namespace ET
{
    public static class SceneType
    {
        public const int Gate = PackageType.Login * 1000 + 2;
        
        // 客户端
        public const int StateSync = PackageType.StateSync * 1000 + 20;
        public const int Current = PackageType.StateSync * 1000 + 21;
        
        public const int All = 0;
        public const int Main = PackageType.Core * 1000 + 1;
        public const int NetInner = PackageType.Core * 1000 + 2;
        public const int NetClient = PackageType.Core * 1000 + 3;
    }
}