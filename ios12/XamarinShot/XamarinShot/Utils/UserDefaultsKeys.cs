
namespace XamarinShot.Utils {
	public static class UserDefaultsKeys {
		// debug support
		public static string ShowSceneViewStats { get; } = "ShowSceneViewStatsKey";
		public static string ShowWireframe { get; } = "ShowWireframe";
		public static string ShowTrackingState { get; } = "ShowTrackingStateKey";
		public static string ShowARDebug { get; } = "ShowARDebugKey";
		public static string ShowPhysicsDebug { get; } = "EnablePhysicsKey";
		public static string ShowNetworkDebug { get; } = "ShowNetworkDebugKey";
		public static string ShowSettingsInGame { get; } = "ShowSettingsInGame";
		public static string ShowARRelocalizationHelp { get; } = "ShowARRelocalizationHelp";
		public static string TrailWidth { get; } = "TrailWidth";
		public static string TrailLength { get; } = "TrailLength";
		public static string ShowProjectileTrail { get; } = "ShowProjectileTrail";
		public static string UseCustomTrail { get; } = "UseCustomTrail";
		public static string TrailShouldNarrow { get; } = "TrailShouldNarrow";
		public static string AllowGameBoardAutoSize { get; } = "AllowGameBoardAutoSize";
		public static string DisableInGameUI { get; } = "DisableInGameUI";

		// settings
		public static string AntialiasingMode { get; } = "AntialiasingMode";
		public static string PeerID { get; } = "PeerIDDefaultsKey";
		public static string SelectedLevel { get; } = "SelectedLevel";
		public static string HasOnboarded { get; } = "HasOnboarded";
		public static string BoardLocatingMode { get; } = "BoardLocatingMode";
		public static string GameRoomMode { get; } = "GameRoomMode";
		public static string UseEncryption { get; } = "UseEncryption";
		public static string AutoFocus { get; } = "AutoFocus";
		public static string Spectator { get; } = "Spectator";

		public static string ShowReset { get; } = "ShowReset";
		public static string ShowClouds { get; } = "ShowClouds";
		public static string ShowFlags { get; } = "ShowFlags";
		public static string ShowRopeSimulation { get; } = "ShowRopeSimulation";
		public static string ShowLOD { get; } = "showLOD";

		public static string MusicVolume { get; } = "MusicVolume";
		public static string EffectsVolume { get; } = "EffectsVolume";
		public static string SynchronizeMusicWithWallClock { get; } = "MusicSyncWithClock";

		public static string ShowThermalState { get; } = "ShowThermalState";
	}
}
