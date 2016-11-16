// class translated from Java
// Credit goes to Charles Hayden http://www.chayden.net/eliza/Eliza.html

using System;
using System.Collections.Generic;

namespace ElizaCore
{
	/// <summary>Eliza key list.</summary>
	/// <remarks>
	/// Eliza key list.
	/// This stores all the keys.
	/// </remarks>
	public class KeyList : List<Key>
	{
		private const long serialVersionUID = 1L;

		/// <summary>Add a new key.</summary>
		/// <remarks>Add a new key.</remarks>
		public virtual void Add(string key, int rank, DecompList decomp)
		{
			Add(new Key(key, rank, decomp));
		}

		/// <summary>Print all the keys.</summary>
		/// <remarks>Print all the keys.</remarks>
		public virtual void Print(int indent)
		{
			for (int i = 0; i < Count; i++)
			{
				Key k = (Key)this[i];
				k.Print(indent);
			}
		}

		/// <summary>Search the key list for a given key.</summary>
		/// <remarks>
		/// Search the key list for a given key.
		/// Return the Key if found, else null.
		/// </remarks>
		internal virtual Key GetKey(string s)
		{
			for (int i = 0; i < Count; i++)
			{
				Key key = (Key)this[i];
				if (s.Equals(key.GetKey()))
				{
					return key;
				}
			}
			return null;
		}

		/// <summary>Break the string s into words.</summary>
		/// <remarks>
		/// Break the string s into words.
		/// For each word, if isKey is true, then push the key
		/// into the stack.
		/// </remarks>
		public virtual void BuildKeyStack(KeyStack stack, string s)
		{
			stack.Reset();
			s = EString.Trim(s);
			string[] lines = new string[2];
			Key k;
			while (EString.Match(s, "* *", lines))
			{
				k = GetKey(lines[0]);
				if (k != null)
				{
					stack.PushKey(k);
				}
				s = lines[1];
			}
			k = GetKey(s);
			if (k != null)
			{
				stack.PushKey(k);
			}
		}
		//stack.print();
	}
}
