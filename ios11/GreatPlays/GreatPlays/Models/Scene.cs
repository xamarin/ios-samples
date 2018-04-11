using System;
using System.Collections.Generic;

namespace GreatPlays {
	public partial class Scene {

		public Act Act { get; set; }
		public int Number { get; set; }
		public Quiz Quiz { get; set; }

		public Scene (int number, Act act)
		{
			Number = number;
			Act = act;
		}
	}
}