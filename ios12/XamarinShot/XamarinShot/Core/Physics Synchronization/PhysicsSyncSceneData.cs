
namespace XamarinShot.Models
{
    using Foundation;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IPhysicsSyncSceneDataDelegate
    {
        void HasNetworkDelayStatusChanged(bool hasNetworkDelay);
       
        Projectile SpawnProjectile(int objectIndex);
       
        void DespawnProjectile(Projectile projectile);

        void PlayPhysicsSound(int objectIndex, CollisionEvent soundEvent);
    }

    public class PhysicsSyncSceneData
    {
        private const int PacketCountToSlowDataUsage = 4;

        private const float NetworkDelayStatusLifetime = 3f;

        private const int MaxPacketCount = 8;

        private readonly NSLock @lock = new NSLock();// need thread protection because add used in main thread, while pack used in render update thread

        // Non-projectile sync
        private readonly List<GameObject> objectList = new List<GameObject>();
        private List<PhysicsNodeData> nodeDataList = new List<PhysicsNodeData>();

        // Projectile sync
        private readonly List<Projectile> projectileList = new List<Projectile>();
        private List<PhysicsNodeData> projectileDataList = new List<PhysicsNodeData>();

        // Sound sync
        private List<CollisionSoundData> soundDataList = new List<CollisionSoundData>();

        // Put data into queue to help with stutters caused by data packet delays
        private readonly List<PhysicsSyncData> packetQueue = new List<PhysicsSyncData>();

        private bool shouldRefillPackets = true;
        private bool justUpdatedHalfway = false;
        private int packetReceived = 0;

        // Network Delay
        private double lastNetworkDelay = 0d;

        // Put up a packet number to make sure that packets are in order
        private int lastPacketNumberRead = 0;

        public IPhysicsSyncSceneDataDelegate Delegate { get; set; }
       
        public bool IsInitialized => this.Delegate != null;

        public bool HasNetworkDelay { get; private set; }

        public void AddObject(GameObject @object)
        {
            var data = @object.GeneratePhysicsData();
            if (data != null)
            {
                this.@lock.Lock();
                this.objectList.Add(@object);
                this.nodeDataList.Add(data);
                this.@lock.Unlock();
            }
        }

        public PhysicsSyncData GenerateData()
        {
            this.@lock.Lock();

            // Update Data of normal nodes
            for (var index = 0; index < this.objectList.Count; index++)
            {
                var data = this.objectList[index].GeneratePhysicsData();
                if (data != null)
                {
                    this.nodeDataList[index] = data;
                }
            }

            // Update Data of projectiles in the pool
            for (var index = 0; index < this.projectileList.Count; index++)
            {
                var data = this.projectileList[index].GeneratePhysicsData();
                if(data != null)
                {
                    this.projectileDataList[index] = data;
                }
            }
           
            // Packet number is used to determined the order of sync data.
            // Because Multipeer Connectivity does not guarantee the order of packet delivery,
            // we use the packet number to discard out of order packets.
            var packetNumber = GameTime.FrameCount % PhysicsSyncData.MaxPacketNumber;
            var packet = new PhysicsSyncData(packetNumber, this.nodeDataList, this.projectileDataList, this.soundDataList);

            // Clear sound data since it only needs to be played once
            this.soundDataList.Clear();
            this.@lock.Unlock();

            return packet;
        }

        public void UpdateFromReceivedData()
        {
            this.@lock.Lock();
            this.DiscardOutOfOrderData();

            if (this.shouldRefillPackets)
            {
                if (this.packetQueue.Count >= MaxPacketCount)
                {
                    this.shouldRefillPackets = false;
                }

                this.@lock.Unlock();
                return;
            }

            var oldestData = this.packetQueue.FirstOrDefault();
            if (oldestData != null)
            {
                // Case when running out of data: Use one packet for two frames
                if (this.justUpdatedHalfway)
                {
                    this.UpdateObjectsFromData(false);
                    this.justUpdatedHalfway = false;
                }
                else if (this.packetQueue.Count <= PacketCountToSlowDataUsage)
                {
                    if (!this.justUpdatedHalfway)
                    {
                        this.Apply(oldestData);
                        this.packetQueue.RemoveAt(0);

                        this.UpdateObjectsFromData(true);
                        this.justUpdatedHalfway = true;
                    }

                    // Case when enough data: Use one packet per frame as usual
                }
                else
                {
                    this.Apply(oldestData);
                    this.packetQueue.RemoveAt(0);
                }
            }
            else
            {
                this.shouldRefillPackets = true;
                //os_log(.info, "out of packets")

                // Update network delay status used to display in sceneViewController
                if (!this.HasNetworkDelay)
                {
                    this.Delegate?.HasNetworkDelayStatusChanged(true);
                }

                this.HasNetworkDelay = true;
                this.lastNetworkDelay = GameTime.Time;
            }

            while (this.packetQueue.Count > MaxPacketCount)
            {
                this.packetQueue.RemoveAt(0);
            }

            // Remove networkDelay status after time passsed without a delay
            if (this.HasNetworkDelay && GameTime.Time - this.lastNetworkDelay > NetworkDelayStatusLifetime)
            {
                this.Delegate?.HasNetworkDelayStatusChanged(false);
                this.HasNetworkDelay = false;
            }

            this.@lock.Unlock();
        }

        public void Receive(PhysicsSyncData packet)
        {
            this.@lock.Lock();

            this.packetQueue.Add(packet);
            this.packetReceived += 1;

            this.@lock.Unlock();
        }

        private void Apply(PhysicsSyncData packet)
        {
            this.lastPacketNumberRead = packet.PacketNumber;
            this.nodeDataList = packet.NodeData;
            this.projectileDataList = packet.ProjectileData;
            this.soundDataList = packet.SoundData;

            // Play sound right away and clear the list
            if (this.Delegate == null)
            {
                throw new Exception("No Delegate");
            }

            foreach (var soundData in this.soundDataList)
            {
                this.Delegate.PlayPhysicsSound(soundData.GameObjectIndex, soundData.SoundEvent);
            }

            this.soundDataList.Clear();
            this.UpdateObjectsFromData(false);
        }

        private void UpdateObjectsFromData(bool isHalfway)
        {
            // Update Nodes
            var objectCount = Math.Min(this.objectList.Count, this.nodeDataList.Count);
            for (var index = 0; index < objectCount; index++)
            {
                if (this.nodeDataList[index].IsAlive)
                {
                    this.objectList[index].Apply(this.nodeDataList[index], isHalfway);
                }
            }

            if (this.Delegate == null)
            {
                throw new Exception("No delegate");
            }

            for (var arrayIndex = 0; arrayIndex < this.projectileList.Count; arrayIndex++)
            {
                var projectile = this.projectileList[arrayIndex];
                var nodeData = this.projectileDataList[arrayIndex];

                // If the projectile must be spawned, spawn it.
                if (nodeData.IsAlive)
                {
                    // Spawn the projectile if it is exists on the other side, but not here
                    if (!projectile.IsAlive)
                    {
                        projectile = this.Delegate.SpawnProjectile(projectile.Index);
                        projectile.Team = nodeData.Team;
                        this.projectileList[arrayIndex] = projectile;
                    }

                    projectile.Apply(nodeData, isHalfway);
                }
                else
                {
                    // Despawn the projectile if it was despawned on the other side
                    if (projectile.IsAlive)
                    {
                        this.Delegate.DespawnProjectile(projectile);
                    }
                }
            }
        }

        private void DiscardOutOfOrderData()
        {
            // Discard data that are out of order
            var oldestData = this.packetQueue.FirstOrDefault();
            while (oldestData != null)
            {
                var packetNumber = oldestData.PacketNumber;
                // If packet number of more than last packet number, then it is in order.
                // For the edge case where packet number resets to 0 again, we test if the difference is more than half the max packet number.
                if (packetNumber > this.lastPacketNumberRead ||
                    (this.lastPacketNumberRead - packetNumber) > PhysicsSyncData.HalfMaxPacketNumber)
                {
                    break;
                }
                else
                {
                    //os_log(.error, "Packet out of order")
                    this.packetQueue.RemoveAt(0);
                }

                oldestData = this.packetQueue.FirstOrDefault();
            }
        }

        #region  Projectile Sync

        public void AddProjectile(Projectile projectile)
        {
            var data = projectile.GeneratePhysicsData();
            if (data != null)
            {
                this.@lock.Lock();
                this.projectileList.Add(projectile);
                this.projectileDataList.Add(data);
                this.@lock.Unlock();
            }
        }

        public void ReplaceProjectile(Projectile projectile)
        {
            this.@lock.Lock();
         
            var oldProjectile = this.projectileList.FirstOrDefault(tile => tile.Index == projectile.Index);
            if (oldProjectile != null)
            {
                this.projectileList[this.projectileList.IndexOf(oldProjectile)] = projectile;
            }
            else
            {
                throw new Exception($"Cannot find the projectile to replace {projectile.Index}");
            }

            this.@lock.Unlock();
        }

        #endregion

        #region Sound Sync

        public void AddSound(int gameObjectIndex, CollisionEvent soundEvent)
        {
            this.soundDataList.Add(new CollisionSoundData(gameObjectIndex, soundEvent));
        }
        
        #endregion
    }
}