using System;
using System.Collections.Generic;
using System.Linq;
using ClassKit;

namespace GreatPlays {
	public partial class Play : INode {
		public INode Parent => null;

		public List<INode> Children => Acts.Cast<INode> ().ToList ();

		public string Identifier => Title;

		public CLSContextType ContextType => CLSContextType.Book;
	}
}
