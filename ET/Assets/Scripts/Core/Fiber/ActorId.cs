using System;
using System.Runtime.InteropServices;
using MemoryPack;
using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    [MemoryPackable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct ActorId
    {
        public bool Equals(ActorId other)
        {
            return this.FiberId == other.FiberId && this.InstanceId == other.InstanceId;
        }

        public override bool Equals(object obj)
        {
            return obj is ActorId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.FiberId, this.InstanceId);
        }

        [MemoryPackOrder(0)]
        public int FiberId;
        [MemoryPackOrder(1)]
        public long InstanceId;

        public ActorId(int fiberId)
        {
            this.FiberId = fiberId;
            this.InstanceId = 1;
        }
        
        public ActorId(int fiberId, long instanceId)
        {
            this.FiberId = fiberId;
            this.InstanceId = instanceId;
        }
        
        public static bool operator ==(ActorId left, ActorId right)
        {
            return left.InstanceId == right.InstanceId && left.FiberId == right.FiberId;
        }

        public static bool operator !=(ActorId left, ActorId right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{this.FiberId}:{this.InstanceId}";
        }
    }
}