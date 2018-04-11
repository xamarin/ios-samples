using System;
using System.Collections.Generic;
using ClassKit;

namespace GreatPlays {
	public partial class Scene : INode {
		public INode Parent => Act;

		public List<INode> Children => Quiz == null ? null : new List<INode> { Quiz };

		public string Identifier => $"Scene {Number}";

		public CLSContextType ContextType => CLSContextType.Section;
	}
}
