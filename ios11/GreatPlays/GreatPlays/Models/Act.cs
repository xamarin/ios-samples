using System;
using System.Collections.Generic;

namespace GreatPlays {
	public partial class Act {	
		public Play Play { get; set; }
		public int Number { get; set; }
		public List<Scene> Scenes { get; set; }

		public Act (int number, Play play)
		{
			Number = number;
			Play = play;
			Scenes = new List<Scene> ();
		}
	}
}