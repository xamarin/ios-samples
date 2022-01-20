
namespace XamarinShot.Models;

public interface IPhysicsSyncSceneDataDelegate
{
        void HasNetworkDelayStatusChanged (bool hasNetworkDelay);

        Projectile SpawnProjectile (int objectIndex);

        void DespawnProjectile (Projectile projectile);

        void PlayPhysicsSound (int objectIndex, CollisionEvent soundEvent);
}

public class PhysicsSyncSceneData
{
        const int PacketCountToSlowDataUsage = 4;

        const float NetworkDelayStatusLifetime = 3f;

        const int MaxPacketCount = 8;

        readonly NSLock @lock = new NSLock ();// need thread protection because add used in main thread, while pack used in render update thread

        // Non-projectile sync
        readonly List<GameObject> objectList = new List<GameObject> ();
        List<PhysicsNodeData> nodeDataList = new List<PhysicsNodeData> ();

        // Projectile sync
        readonly List<Projectile> projectileList = new List<Projectile> ();
        List<PhysicsNodeData> projectileDataList = new List<PhysicsNodeData> ();

        // Sound sync
        List<CollisionSoundData> soundDataList = new List<CollisionSoundData> ();

        // Put data into queue to help with stutters caused by data packet delays
        readonly List<PhysicsSyncData> packetQueue = new List<PhysicsSyncData> ();

        bool shouldRefillPackets = true;
        bool justUpdatedHalfway = false;
        int packetReceived = 0;

        // Network Delay
        double lastNetworkDelay = 0d;

        // Put up a packet number to make sure that packets are in order
        int lastPacketNumberRead = 0;

        public IPhysicsSyncSceneDataDelegate? Delegate { get; set; }

        public bool IsInitialized => Delegate is not null;

        public bool HasNetworkDelay { get; private set; }

        public void AddObject (GameObject @object)
        {
                var data = @object.GeneratePhysicsData ();
                if (data is not null)
                {
                        @lock.Lock ();
                        objectList.Add (@object);
                        nodeDataList.Add (data);
                        @lock.Unlock ();
                }
        }

        public PhysicsSyncData GenerateData ()
        {
                @lock.Lock ();

                // Update Data of normal nodes
                for (var index = 0; index < objectList.Count; index++)
                {
                        var data = objectList [index].GeneratePhysicsData ();
                        if (data is not null)
                        {
                                nodeDataList [index] = data;
                        }
                }

                // Update Data of projectiles in the pool
                for (var index = 0; index < projectileList.Count; index++)
                {
                        var data = projectileList [index].GeneratePhysicsData ();
                        if (data is not null)
                        {
                                projectileDataList [index] = data;
                        }
                }

                // Packet number is used to determined the order of sync data.
                // Because Multipeer Connectivity does not guarantee the order of packet delivery,
                // we use the packet number to discard out of order packets.
                var packetNumber = GameTime.FrameCount % PhysicsSyncData.MaxPacketNumber;
                var packet = new PhysicsSyncData (packetNumber, nodeDataList, projectileDataList, soundDataList);

                // Clear sound data since it only needs to be played once
                soundDataList.Clear ();
                @lock.Unlock ();

                return packet;
        }

        public void UpdateFromReceivedData ()
        {
                @lock.Lock ();
                DiscardOutOfOrderData ();

                if (shouldRefillPackets)
                {
                        if (packetQueue.Count >= MaxPacketCount)
                        {
                                shouldRefillPackets = false;
                        }

                        @lock.Unlock ();
                        return;
                }

                var oldestData = packetQueue.FirstOrDefault ();
                if (oldestData is not null)
                {
                        // Case when running out of data: Use one packet for two frames
                        if (justUpdatedHalfway)
                        {
                                UpdateObjectsFromData (false);
                                justUpdatedHalfway = false;
                        } else if (packetQueue.Count <= PacketCountToSlowDataUsage) {
                                if (!justUpdatedHalfway)
                                {
                                        Apply (oldestData);
                                        packetQueue.RemoveAt (0);

                                        UpdateObjectsFromData (true);
                                        justUpdatedHalfway = true;
                                }

                                // Case when enough data: Use one packet per frame as usual
                        } else {
                                Apply (oldestData);
                                packetQueue.RemoveAt (0);
                        }
                } else {
                        shouldRefillPackets = true;
                        //os_log(.info, "out of packets")

                        // Update network delay status used to display in sceneViewController
                        if (!HasNetworkDelay)
                        {
                                Delegate?.HasNetworkDelayStatusChanged (true);
                        }

                        HasNetworkDelay = true;
                        lastNetworkDelay = GameTime.Time;
                }

                while (packetQueue.Count > MaxPacketCount)
                {
                        packetQueue.RemoveAt (0);
                }

                // Remove networkDelay status after time passsed without a delay
                if (HasNetworkDelay && GameTime.Time - lastNetworkDelay > NetworkDelayStatusLifetime)
                {
                        Delegate?.HasNetworkDelayStatusChanged (false);
                        HasNetworkDelay = false;
                }

                @lock.Unlock ();
        }

        public void Receive (PhysicsSyncData packet)
        {
                @lock.Lock ();

                packetQueue.Add (packet);
                packetReceived += 1;

                @lock.Unlock ();
        }

        void Apply (PhysicsSyncData packet)
        {
                lastPacketNumberRead = packet.PacketNumber;
                nodeDataList = packet.NodeData;
                projectileDataList = packet.ProjectileData;
                soundDataList = packet.SoundData;

                // Play sound right away and clear the list
                if (Delegate is null)
                {
                        throw new Exception ("No Delegate");
                }

                foreach (var soundData in soundDataList)
                {
                        Delegate.PlayPhysicsSound (soundData.GameObjectIndex, soundData.SoundEvent);
                }

                soundDataList.Clear ();
                UpdateObjectsFromData (false);
        }

        void UpdateObjectsFromData (bool isHalfway)
        {
                // Update Nodes
                var objectCount = Math.Min (objectList.Count, nodeDataList.Count);
                for (var index = 0; index < objectCount; index++)
                {
                        if (nodeDataList [index].IsAlive)
                        {
                                objectList [index].Apply (nodeDataList [index], isHalfway);
                        }
                }

                if (Delegate is null)
                {
                        throw new Exception ("No delegate");
                }

                for (var arrayIndex = 0; arrayIndex < projectileList.Count; arrayIndex++)
                {
                        var projectile = projectileList [arrayIndex];
                        var nodeData = projectileDataList [arrayIndex];

                        // If the projectile must be spawned, spawn it.
                        if (nodeData.IsAlive)
                        {
                                // Spawn the projectile if it is exists on the other side, but not here
                                if (!projectile.IsAlive)
                                {
                                        projectile = Delegate.SpawnProjectile (projectile.Index);
                                        projectile.Team = nodeData.Team;
                                        projectileList [arrayIndex] = projectile;
                                }

                                projectile.Apply (nodeData, isHalfway);
                        } else {
                                // Despawn the projectile if it was despawned on the other side
                                if (projectile.IsAlive)
                                {
                                        Delegate.DespawnProjectile (projectile);
                                }
                        }
                }
        }

        void DiscardOutOfOrderData ()
        {
                // Discard data that are out of order
                var oldestData = packetQueue.FirstOrDefault ();
                while (oldestData is not null)
                {
                        var packetNumber = oldestData.PacketNumber;
                        // If packet number of more than last packet number, then it is in order.
                        // For the edge case where packet number resets to 0 again, we test if the difference is more than half the max packet number.
                        if (packetNumber > lastPacketNumberRead ||
                                (lastPacketNumberRead - packetNumber) > PhysicsSyncData.HalfMaxPacketNumber)
                        {
                                break;
                        } else {
                                //os_log(.error, "Packet out of order")
                                packetQueue.RemoveAt (0);
                        }

                        oldestData = packetQueue.FirstOrDefault ();
                }
        }

        #region  Projectile Sync

        public void AddProjectile (Projectile projectile)
        {
                var data = projectile.GeneratePhysicsData ();
                if (data is not null)
                {
                        @lock.Lock ();
                        projectileList.Add (projectile);
                        projectileDataList.Add (data);
                        @lock.Unlock ();
                }
        }

        public void ReplaceProjectile (Projectile projectile)
        {
                @lock.Lock ();

                var oldProjectile = projectileList.FirstOrDefault (tile => tile.Index == projectile.Index);
                if (oldProjectile is not null)
                {
                        projectileList [projectileList.IndexOf (oldProjectile)] = projectile;
                } else {
                        throw new Exception ($"Cannot find the projectile to replace {projectile.Index}");
                }

                @lock.Unlock ();
        }

        #endregion

        #region Sound Sync

        public void AddSound (int gameObjectIndex, CollisionEvent soundEvent)
        {
                soundDataList.Add (new CollisionSoundData (gameObjectIndex, soundEvent));
        }

        #endregion
}
