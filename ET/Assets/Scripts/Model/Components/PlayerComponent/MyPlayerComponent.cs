namespace ET.Client
{
    //客户端用来管理本地Player的Id的组建
    [ComponentOf(typeof(Scene))]
    public class MyPlayerComponent: Entity, IAwake
    {
        public long MyId { get; set; }
    }
}