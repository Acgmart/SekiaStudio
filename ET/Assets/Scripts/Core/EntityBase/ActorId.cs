using System;
using System.Runtime.InteropServices;
using MemoryPack;
using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
    [MemoryPackable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct Address
    {
        [MemoryPackOrder(0)]
        public int Fiber;
        
        public bool Equals(Address other)
        {
            return this.Fiber == other.Fiber;
        }

        public override bool Equals(object obj)
        {
            return obj is Address other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Fiber);
        }
        
        public Address(int fiber)
        {
            this.Fiber = fiber;
        }

        public static bool operator ==(Address left, Address right)
        {
            return left.Fiber == right.Fiber;
        }

        public static bool operator !=(Address left, Address right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{this.Fiber}";
        }
    }
    
    [MemoryPackable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct ActorId
    {
        public bool Equals(ActorId other)
        {
            return this.Address == other.Address && this.InstanceId == other.InstanceId;
        }

        public override bool Equals(object obj)
        {
            return obj is ActorId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Address, this.InstanceId);
        }

        [MemoryPackOrder(0)]
        public Address Address;
        [MemoryPackOrder(1)]
        public long InstanceId;

        [BsonIgnore]
        public int Fiber
        {
            get
            {
                return this.Address.Fiber;
            }
            set
            {
                this.Address.Fiber = value;
            }
        }
        
        public ActorId(int fiber)
        {
            this.Address = new Address(fiber);
            this.InstanceId = 1;
        }
        
        public ActorId(int fiber, long instanceId)
        {
            this.Address = new Address(fiber);
            this.InstanceId = instanceId;
        }
        
        public ActorId(Address address): this(address, 1)
        {
        }
        
        public ActorId(Address address, long instanceId)
        {
            this.Address = address;
            this.InstanceId = instanceId;
        }

        public static bool operator ==(ActorId left, ActorId right)
        {
            return left.InstanceId == right.InstanceId && left.Address == right.Address;
        }

        public static bool operator !=(ActorId left, ActorId right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{this.Fiber}:{this.InstanceId}";
        }
    }
}