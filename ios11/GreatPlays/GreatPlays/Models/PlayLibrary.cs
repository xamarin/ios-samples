using System;
using System.Collections.Generic;
using Foundation;

namespace GreatPlays {
	public partial class PlayLibrary : NSObject {
		public static PlayLibrary Shared { get; } = new PlayLibrary ();
		public List<Play> Plays { get; private set; } = new List<Play> ();

		private PlayLibrary ()
		{
			SetupClassKit ();
		}

		public void AddPlay (Play play)
		{
			Plays.Add (play);
			SetupContext (play);
		}
	}
}
