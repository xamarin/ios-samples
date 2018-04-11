using System;
using System.Collections.Generic;

namespace GreatPlays {
	public partial class Play {
		public string Title { get; set; }
		public List<Act> Acts { get; set; }

		public Play (string title)
		{
			Title = title;
			Acts = new List<Act> ();
		}
	}
}
