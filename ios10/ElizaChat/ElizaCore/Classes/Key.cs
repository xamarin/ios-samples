// class translated from Java
// Credit goes to Charles Hayden http://www.chayden.net/eliza/Eliza.html

using System;

namespace ElizaCore
{
	/// <summary>Eliza key.</summary>
	/// <remarks>
	/// Eliza key.
	/// A key has the key itself, a rank, and a list of decompositon rules.
	/// </remarks>
	public class Key
	{
		/// <summary>The key itself</summary>
		internal string key;

		/// <summary>The numerical rank</summary>
		internal int rank;

		/// <summary>The list of decompositions</summary>
		internal DecompList decomp;

		/// <summary>Initialize the key.</summary>
		/// <remarks>Initialize the key.</remarks>
		internal Key(string key, int rank, DecompList decomp)
		{
			this.key = key;
			this.rank = rank;
			this.decomp = decomp;
		}

		/// <summary>Another initialization for gotoKey.</summary>
		/// <remarks>Another initialization for gotoKey.</remarks>
		internal Key()
		{
			key = null;
			rank = 0;
			decomp = null;
		}

		public virtual void Copy(Key k)
		{
			key = k.GetKey();
			rank = k.Rank();
			decomp = k.Decomp();
		}

		/// <summary>Print the key and all under it.</summary>
		/// <remarks>Print the key and all under it.</remarks>
		public virtual void Print(int indent)
		{
			for (int i = 0; i < indent; i++)
			{
				ConsoleSurrogate.Write(" ");
			}
			ConsoleSurrogate.WriteLine("key: " + key + " " + rank);
			decomp.Print(indent + 2);
		}

		/// <summary>Print the key and rank only, not the rest.</summary>
		/// <remarks>Print the key and rank only, not the rest.</remarks>
		public virtual void PrintKey(int indent)
		{
			for (int i = 0; i < indent; i++)
			{
				ConsoleSurrogate.Write(" ");
			}
			ConsoleSurrogate.WriteLine("key: " + key + " " + rank);
		}

		/// <summary>Get the key value.</summary>
		/// <remarks>Get the key value.</remarks>
		public virtual string GetKey()
		{
			return key;
		}

		/// <summary>Get the rank.</summary>
		/// <remarks>Get the rank.</remarks>
		public virtual int Rank()
		{
			return rank;
		}

		/// <summary>Get the decomposition list.</summary>
		/// <remarks>Get the decomposition list.</remarks>
		public virtual DecompList Decomp()
		{
			return decomp;
		}
	}
}
