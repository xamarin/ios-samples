// class translated from Java
// Credit goes to Charles Hayden http://www.chayden.net/eliza/Eliza.html

using System;
using System.Collections.Generic;

namespace ElizaCore {
	/// <summary>Eliza prePost list.</summary>
	/// <remarks>
	/// Eliza prePost list.
	/// This list of pre-post entries is used to perform word transformations
	/// prior to or after other processing.
	/// </remarks>
	public class PrePostList : List<PrePost> {
		private const long serialVersionUID = 1L;

		/// <summary>Add another entry to the list.</summary>
		/// <remarks>Add another entry to the list.</remarks>
		public virtual void Add (string src, string dest)
		{
			Add (new PrePost (src, dest));
		}

		/// <summary>Prnt the pre-post list.</summary>
		/// <remarks>Prnt the pre-post list.</remarks>
		public virtual void Print (int indent)
		{
			for (int i = 0; i < Count; i++) {
				PrePost p = (PrePost) this [i];
				p.Print (indent);
			}
		}

		/// <summary>Translate a string.</summary>
		/// <remarks>
		/// Translate a string.
		/// If str matches a src string on the list,
		/// return he corresponding dest.
		/// If no match, return the input.
		/// </remarks>
		internal virtual string Xlate (string str)
		{
			for (int i = 0; i < Count; i++) {
				PrePost p = (PrePost) this [i];
				if (str.Equals (p.Src ())) {
					return p.Dest ();
				}
			}
			return str;
		}

		/// <summary>Translate a string s.</summary>
		/// <remarks>
		/// Translate a string s.
		/// (1) Trim spaces off.
		/// (2) Break s into words.
		/// (3) For each word, substitute matching src word with dest.
		/// </remarks>
		public virtual string Translate (string s)
		{
			string [] lines = new string [2];
			string work = EString.Trim (s);
			s = string.Empty;
			while (EString.Match (work, "* *", lines)) {
				s += Xlate (lines [0]) + " ";
				work = EString.Trim (lines [1]);
			}
			s += Xlate (work);
			return s;
		}
	}
}
