using System;
using System.Collections.Generic;
using System.Linq;
using ClassKit;

namespace GreatPlays {
	public partial class Act : INode {
		public INode Parent => Play;

		public List<INode> Children => Scenes.Cast<INode> ().ToList ();

		public string Identifier => $"Act {Number}";

		public CLSContextType ContextType => CLSContextType.Chapter;
	}
}
