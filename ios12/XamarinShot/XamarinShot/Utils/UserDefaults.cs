
namespace XamarinShot.Utils
{
    using Foundation;
    using MultipeerConnectivity;
    using XamarinShot.Models;
    using XamarinShot.Models.Enums;
    using System.Collections.Generic;
    using UIKit;

    public static class UserDefaults
    {
        public static Dictionary<string, object> ApplicationDefaults = new Dictionary<string, object>
        {
            { UserDefaultsKeys.Spectator, false },
            { UserDefaultsKeys.MusicVolume, 0f },
            { UserDefaultsKeys.EffectsVolume, 1f },
            { UserDefaultsKeys.AntialiasingMode, true},
            { UserDefaultsKeys.UseEncryption, true},
            { UserDefaultsKeys.GameRoomMode, false},
            { UserDefaultsKeys.AutoFocus, true},
            { UserDefaultsKeys.AllowGameBoardAutoSize, false},
            { UserDefaultsKeys.ShowReset, false},
            { UserDefaultsKeys.ShowFlags, true},
            { UserDefaultsKeys.ShowClouds, false},
            { UserDefaultsKeys.SynchronizeMusicWithWallClock, true},
            { UserDefaultsKeys.ShowRopeSimulation, true},
            { UserDefaultsKeys.ShowThermalState, true},
            { UserDefaultsKeys.ShowProjectileTrail, true},
            { UserDefaultsKeys.TrailShouldNarrow, true },
        };

        public static Player Myself
        {
            get
            {
                Player result = null;
                if (NSUserDefaults.StandardUserDefaults[UserDefaultsKeys.PeerID] is NSData data)
                {
                    if (NSKeyedUnarchiver.GetUnarchivedObject(typeof(MCPeerID), data, out NSError error) is MCPeerID peerID)
                    {
                        result =  new Player(peerID);
                    }
                }

                if (result == null)
                {
                    // if no playerID was previously selected, create and cache a new one.
                    result = new Player(UIDevice.CurrentDevice.Name);
                    // update inner value
                    Myself = result;
                }

                return result;
            }

            set
            {
                var data = NSKeyedArchiver.ArchivedDataWithRootObject(value.PeerId, true, out NSError archivedError);
                NSUserDefaults.StandardUserDefaults[UserDefaultsKeys.PeerID] = NSObject.FromObject(data);
            }
        }

        public static float MusicVolume
        {
            get
            {

                return NSUserDefaults.StandardUserDefaults.FloatForKey(UserDefaultsKeys.MusicVolume);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetFloat(value, UserDefaultsKeys.MusicVolume);
            }
        }

        public static float EffectsVolume
        {
            get
            {

                return NSUserDefaults.StandardUserDefaults.FloatForKey(UserDefaultsKeys.EffectsVolume);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetFloat(value, UserDefaultsKeys.EffectsVolume);
            }
        }

        public static bool ShowARDebug
        {
            get
            {

                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowARDebug);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowARDebug);
            }
        }

        public static bool ShowSceneViewStats
        {
            get
            {

                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowSceneViewStats);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowSceneViewStats);
            }
        }

        /// <summary>
        /// This is a wireframe overlay for looking at poly-count (f.e. LOD)
        /// </summary>
        public static bool ShowWireframe
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowWireframe);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowWireframe);
            }
        }

        /// <summary>
        /// This turns shapes emissive channel red (set at level start)
        /// </summary>
        public static bool ShowLOD
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowLOD);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowLOD);
            }
        }

        /// <summary>
        /// This may need to be integer for 0, 2, 4x
        /// </summary>
        public static bool AntialiasingMode
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.AntialiasingMode);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.AntialiasingMode);
            }
        }

        public static bool ShowTrackingState
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowTrackingState);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowTrackingState);
            }
        }

        public static GameLevel SelectedLevel
        {
            get
            {
                var levelName = NSUserDefaults.StandardUserDefaults.StringForKey(UserDefaultsKeys.SelectedLevel);
                if (!string.IsNullOrEmpty(levelName))
                {
                    var level = GameLevel.Level(levelName);
                    return level ?? GameLevel.DefaultLevel;
                }
                else
                {
                    return GameLevel.DefaultLevel;
                }
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetString(value.Name, UserDefaultsKeys.SelectedLevel);
            }
        }

        public static bool ShowPhysicsDebug
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowPhysicsDebug);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowPhysicsDebug);
            }
        }

        public static bool ShowNetworkDebug
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowNetworkDebug);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowNetworkDebug);
            }
        }

        public static bool HasOnboarded
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.HasOnboarded);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.HasOnboarded);
            }
        }

        public static BoardLocatingMode BoardLocatingMode
        {
            get
            {
                var value = NSUserDefaults.StandardUserDefaults.IntForKey(UserDefaultsKeys.BoardLocatingMode);
                return (BoardLocatingMode)(int)value;
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetInt((int)value, UserDefaultsKeys.BoardLocatingMode);
            }
        }

        public static bool GameRoomMode
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.GameRoomMode);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.GameRoomMode);
            }
        }

        public static bool UseEncryption
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.UseEncryption);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.UseEncryption);
            }
        }

        public static bool ShowSettingsInGame
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowSettingsInGame);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowSettingsInGame);
            }
        }

        public static bool ShowARRelocalizationHelp
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowARRelocalizationHelp);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowARRelocalizationHelp);
            }
        }

        public static int? TrailLength
        {
            get
            {
                var value = NSUserDefaults.StandardUserDefaults[UserDefaultsKeys.TrailLength] as NSNumber;
                return value?.Int32Value;
            }
            set
            {
                if (value.HasValue)
                {
                    NSUserDefaults.StandardUserDefaults.SetInt(value.Value, UserDefaultsKeys.TrailLength);
                }
                else
                {
                    NSUserDefaults.StandardUserDefaults.RemoveObject(UserDefaultsKeys.TrailLength);
                }
            }
        }

        public static float? TrailWidth
        {
            get
            {
                var value = NSUserDefaults.StandardUserDefaults[UserDefaultsKeys.TrailWidth] as NSNumber;
                return value?.FloatValue;
            }
            set
            {
                if (value.HasValue)
                {
                    NSUserDefaults.StandardUserDefaults[UserDefaultsKeys.TrailWidth] = new NSNumber(value.Value);
                }
                else
                {
                    NSUserDefaults.StandardUserDefaults.RemoveObject(UserDefaultsKeys.TrailWidth);
                }
            }
        }

        public static bool ShowProjectileTrail
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowProjectileTrail);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowProjectileTrail);
            }
        }

        public static bool UseCustomTrail
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.UseCustomTrail);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.UseCustomTrail);
            }
        }

        public static bool TrailShouldNarrow
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.TrailShouldNarrow);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.TrailShouldNarrow);
            }
        }

        public static bool ShowResetLever
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowReset);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowReset);
            }
        }

        public static bool ShowClouds
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowClouds);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowClouds);
            }
        }

        public static bool ShowFlags
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowFlags);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowFlags);
            }
        }

        public static bool ShowRopeSimulation
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowRopeSimulation);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowRopeSimulation);
            }
        }

        public static bool AutoFocus
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.AutoFocus);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.AutoFocus);
            }
        }

        public static bool AllowGameBoardAutoSize
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.AllowGameBoardAutoSize);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.AllowGameBoardAutoSize);
            }
        }

        public static bool Spectator
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.Spectator);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.Spectator);
            }
        }

        public static bool DisableInGameUI
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.DisableInGameUI);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.DisableInGameUI);
            }
        }

        public static bool SynchronizeMusicWithWallClock
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.SynchronizeMusicWithWallClock);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.SynchronizeMusicWithWallClock);
            }
        }

        public static bool ShowThermalState
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.BoolForKey(UserDefaultsKeys.ShowThermalState);
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetBool(value, UserDefaultsKeys.ShowThermalState);
            }
        }
    }
}