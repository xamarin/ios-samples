
namespace XamarinShot.Models
{
    using System;
    using System.Collections.Generic;

    public class PhysicsSyncData
    {
        public PhysicsSyncData(int packetNumber,
                               List<PhysicsNodeData> nodeData,
                               List<PhysicsNodeData> projectileData,
                               List<CollisionSoundData> soundData)
        {
            this.PacketNumber = packetNumber;
            this.NodeData = nodeData;
            this.ProjectileData = projectileData;
            this.SoundData = soundData;
        }

        public int PacketNumber { get; set; }

        public List<PhysicsNodeData> NodeData { get; set; }

        public List<PhysicsNodeData> ProjectileData { get; set; }

        public List<CollisionSoundData> SoundData { get; set; }

        public static int PacketNumberBits = 12; // 12 bits represents packetNumber reset every minute

        public static int NodeCountBits = 9;

        public static int MaxPacketNumber = (int)Math.Pow(2d, PacketNumberBits);

        public static int HalfMaxPacketNumber = MaxPacketNumber / 2;
    }
}