namespace ET.Server
{
    [EntitySystemOf(typeof(Player))]
    public static partial class PlayerSystem
    {
        [EntitySystem]
        private static void Awake(this Player self, string a)
        {
            self.Account = a;
        }
    }
    
    [ChildOf(typeof(PlayerComponent))]
    public sealed class Player : Entity, IAwake<string>
    {
        public string Account { get; set; }
    }
}