// class translated from Java
// Credit goes to Charles Hayden http://www.chayden.net/eliza/Eliza.html

using System.Collections.Generic;


namespace ElizaCore {
	/// <summary>Eliza reassembly list.</summary>
	/// <remarks>Eliza reassembly list.</remarks>
	public class ReasembList : List<string> {
		private const long serialVersionUID = 1L;

		/// <summary>Print the reassembly list.</summary>
		/// <remarks>Print the reassembly list.</remarks>
		public virtual void Print (int indent)
		{
			for (int i = 0; i < Count; i++) {
				for (int j = 0; j < indent; j++) {
					ConsoleSurrogate.Write (" ");
				}
				string s = (string) this [i];
				ConsoleSurrogate.WriteLine ("reasemb: " + s);
			}
		}
	}
}
