using System;
using GameKit;

namespace ButtonTapper3000 {

	public enum GameMode {
		Easy,
		Hard,
		Max
	}

	public enum GameTime {
		Fifteen = 0,
		Thirty,
		FourtyFive,
		Max
	}

	public enum GameTypePlayed {
		Invalid = -0x0001,
		Easy15 = 0x0001,
		Hard15 = 0x0002,
		Easy30 = 0x0004,
		Hard30 = 0x0008,
		Easy45 = 0x0010,
		Hard45 = 0x0020,
		All = 0x003F
	}

	public static class GameInfo {

		public static GameMode GameMode;

		static GameTime gameTime;
		public static GameTime GameTime {
			get { return gameTime; }
			set {
				gameTime = value;
				CurrentTicks = GameTimeInSeconds;
			}
		}

		public static int CurrentTaps;
		public static GKChallenge Challenge;
		public static GKLeaderboardSet CurrentSet;
		public static GKLeaderboard CurrentLeaderBoard;
		public static int[] GameTimes;
		public static int CurrentTicks;

		static GameInfo ()
		{
			GameMode = GameMode.Easy;
			GameTimes = new [] { 15, 30, 45 };
			GameTime = GameTime.Fifteen;
			CurrentTaps = 0;
			CurrentTicks = GameTimes [(int)GameTime];
		}

		public static int GameTimeInSeconds {
			get {
				return GameTimes [(int)GameTime];
			}
		}

		public static void ResetGame ()
		{
			CurrentTaps = 0;
			Challenge = null;
			CurrentTicks = GameTimes [(int)GameTime];
		}
	}
}