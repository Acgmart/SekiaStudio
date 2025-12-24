using Unity.Mathematics;

namespace ET.Client
{
    [FriendOf(typeof(XunLuoPathComponent))]
    public static partial class XunLuoPathComponentSystem
    {
        public static float3 GetCurrent(this XunLuoPathComponent self)
        {
            return self.path[self.Index];
        }
        
        public static void MoveNext(this XunLuoPathComponent self)
        {
            self.Index = ++self.Index % self.path.Length;
        }
    }
    
    [ComponentOf(typeof(Unit))]
    public class XunLuoPathComponent: Entity, IAwake
    {
        public float3[] path = new float3[] { new float3(0, 0, 0), new float3(20, 0, 0), new float3(20, 0, 20), new float3(0, 0, 20), };
        public int Index;
    }
}