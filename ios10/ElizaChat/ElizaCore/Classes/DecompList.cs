// class translated from Java
// Credit goes to Charles Hayden http://www.chayden.net/eliza/Eliza.html

using System.Collections.Generic;

namespace ElizaCore {
	/// <summary>Eliza decomp list.</summary>
	/// <remarks>
	/// Eliza decomp list.
	/// This stores all the decompositions of a single key.
	/// </remarks>
	public class DecompList : List<Decomp> {
		/// <summary>Add another decomp rule to the list.</summary>
		/// <remarks>Add another decomp rule to the list.</remarks>
		public virtual void Add (string word, bool mem, ReasembList reasmb)
		{
			Add (new Decomp (word, mem, reasmb));
		}

		/// <summary>Print the whole decomp list.</summary>
		/// <remarks>Print the whole decomp list.</remarks>
		public virtual void Print (int indent)
		{
			for (int i = 0; i < Count; i++) {
				Decomp d = (Decomp) this [i];
				d.Print (indent);
			}
		}
	}
}
