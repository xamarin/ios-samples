using System;
using System.Collections.Generic;
using ClassKit;

namespace GreatPlays {
	public partial class Quiz : INode {
		public INode Parent => Scene;

		public List<INode> Children => null;

		public string Identifier => Title;

		public CLSContextType ContextType => CLSContextType.Quiz;
	}
}
