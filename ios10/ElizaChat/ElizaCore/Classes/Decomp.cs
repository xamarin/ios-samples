// class translated from Java
// Credit goes to Charles Hayden http://www.chayden.net/eliza/Eliza.html

using System;

namespace ElizaCore {
	/// <summary>Eliza decomposition rule</summary>
	public class Decomp {
		/// <summary>The decomp pattern</summary>
		internal string pattern;

		/// <summary>The mem flag</summary>
		internal bool mem;

		/// <summary>The reassembly list</summary>
		internal ReasembList reasemb;

		/// <summary>The current reassembly point</summary>
		internal int currReasmb;

		/// <summary>Initialize the decomp rule</summary>
		internal Decomp (string pattern, bool mem, ReasembList reasemb)
		{
			this.pattern = pattern;
			this.mem = mem;
			this.reasemb = reasemb;
			this.currReasmb = 100;
		}

		/// <summary>Print out the decomp rule.</summary>
		/// <remarks>Print out the decomp rule.</remarks>
		public virtual void Print (int indent)
		{
			string m = mem ? "true" : "false";
			for (int i = 0; i < indent; i++) {
				ConsoleSurrogate.Write (" ");
			}
			ConsoleSurrogate.WriteLine ("decomp: " + pattern + " " + m);
			reasemb.Print (indent + 2);
		}

		/// <summary>Get the pattern.</summary>
		/// <remarks>Get the pattern.</remarks>
		public virtual string Pattern ()
		{
			return pattern;
		}

		/// <summary>Get the mem flag.</summary>
		/// <remarks>Get the mem flag.</remarks>
		public virtual bool Mem ()
		{
			return mem;
		}

		/// <summary>Get the next reassembly rule.</summary>
		/// <remarks>Get the next reassembly rule.</remarks>
		public virtual string NextRule ()
		{
			if (reasemb.Count == 0) {
				ConsoleSurrogate.WriteLine ("No reassembly rule.");
				return null;
			}
			return (string) reasemb [currReasmb];
		}

		/// <summary>Step to the next reassembly rule.</summary>
		/// <remarks>
		/// Step to the next reassembly rule.
		/// If mem is true, pick a random rule.
		/// </remarks>
		public virtual void StepRule ()
		{
			int size = reasemb.Count;
			if (mem) {
				currReasmb = (int) (new Random ().Next () * size);
			}
			//  Increment and make sure it is within range.
			currReasmb++;
			if (currReasmb >= size) {
				currReasmb = 0;
			}
		}
	}
}
