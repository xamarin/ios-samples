// class translated from Java
// Credit goes to Charles Hayden http://www.chayden.net/eliza/Eliza.html

using System;
using System.Collections.Generic;


namespace ElizaCore {
	/// <summary>Eliza synonym list.</summary>
	/// <remarks>
	/// Eliza synonym list.
	/// Collection of all the synonym elements.
	/// </remarks>
	public class SynList : List<WordList> {
		private const long serialVersionUID = 1L;

		/// <summary>Prnt the synonym lists.</summary>
		/// <remarks>Prnt the synonym lists.</remarks>
		public virtual void Print (int indent)
		{
			for (int i = 0; i < Count; i++) {
				for (int j = 0; j < indent; j++) {
					ConsoleSurrogate.Write (" ");
				}
				ConsoleSurrogate.Write ("synon: ");
				WordList w = (WordList) this [i];
				w.Print (indent);
			}
		}

		/// <summary>Find a synonym word list given the any word in it.</summary>
		/// <remarks>Find a synonym word list given the any word in it.</remarks>
		public virtual WordList Find (string s)
		{
			for (int i = 0; i < Count; i++) {
				WordList w = (WordList) this [i];
				if (w.Find (s)) {
					return w;
				}
			}
			return null;
		}

		/// <summary>
		/// Decomposition match,
		/// If decomp has no synonyms, do a regular match.
		/// </summary>
		/// <remarks>
		/// Decomposition match,
		/// If decomp has no synonyms, do a regular match.
		/// Otherwise, try all synonyms.
		/// </remarks>
		internal virtual bool MatchDecomp (string str, string pat, string [] lines)
		{
			if (!EString.Match (pat, "*@* *", lines)) {
				//  no synonyms in decomp pattern
				return EString.Match (str, pat, lines);
			}
			//  Decomp pattern has synonym -- isolate the synonym
			string first = lines [0];
			string synWord = lines [1];
			string theRest = " " + lines [2];
			//  Look up the synonym
			WordList syn = Find (synWord);
			if (syn == null) {
				ConsoleSurrogate.WriteLine ("Could not fnd syn list for " + synWord);
				return false;
			}
			//  Try each synonym individually
			for (int i = 0; i < syn.Count; i++) {
				//  Make a modified pattern
				pat = first + (string) syn [i] + theRest;
				if (EString.Match (str, pat, lines)) {
					int n = EString.Count (first, '*');
					//  Make room for the synonym in the match list.
					for (int j = lines.Length - 2; j >= n; j--) {
						lines [j + 1] = lines [j];
					}
					//  The synonym goes in the match list.
					lines [n] = (string) syn [i];
					return true;
				}
			}
			return false;
		}
	}
}
