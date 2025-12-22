namespace ET
{
    public static class MailBoxType
    {
        public const int OrderedMessage = PackageType.ActorLocation * 1000 + 1;
        public const int GateSession = PackageType.Login * 1000 + 1;
        public const int UnOrderedMessage = PackageType.Core * 1000 + 1;
    }
}