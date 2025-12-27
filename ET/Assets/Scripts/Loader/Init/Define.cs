namespace ET
{
    public static class Define
    {
#if DEBUG
        public static bool IsDebug = true;
#else
        public static bool IsDebug = false;
#endif

#if UNITY_EDITOR && !ASYNC
        public static bool IsAsync = false;
#else
        public static bool IsAsync = true;
#endif

#if UNITY_EDITOR
        public static bool IsEditor = true;
#else
        public static bool IsEditor = false;
#endif

#if ENABLE_VIEW
        public static bool EnableView = true;
#else
        public static bool EnableView = false;
#endif

#if ENABLE_IL2CPP
        public static bool EnableIL2CPP = true;
#else
        public static bool EnableIL2CPP = false;
#endif
    }
}