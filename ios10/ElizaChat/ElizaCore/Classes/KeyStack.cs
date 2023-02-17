// class translated from Java
// Credit goes to Charles Hayden http://www.chayden.net/eliza/Eliza.html

namespace ElizaCore {
	/// <summary>A stack of keys.</summary>
	/// <remarks>
	/// A stack of keys.
	/// The keys are kept in rank order.
	/// </remarks>
	public class KeyStack {
		/// <summary>The stack size</summary>
		internal static readonly int stackSize = 20;

		/// <summary>The key stack</summary>
		internal Key [] keyStack = new Key [stackSize];

		/// <summary>The top of the key stack</summary>
		internal int keyTop = 0;

		/// <summary>Prints the key stack.</summary>
		/// <remarks>Prints the key stack.</remarks>
		public virtual void Print ()
		{
			ConsoleSurrogate.WriteLine ("Key stack " + keyTop);
			for (int i = 0; i < keyTop; i++) {
				keyStack [i].PrintKey (0);
			}
		}

		/// <summary>Get the stack size.</summary>
		/// <remarks>Get the stack size.</remarks>
		public virtual int KeyTop ()
		{
			return keyTop;
		}

		/// <summary>Reset the key stack.</summary>
		/// <remarks>Reset the key stack.</remarks>
		public virtual void Reset ()
		{
			keyTop = 0;
		}

		/// <summary>Get a key from the stack.</summary>
		/// <remarks>Get a key from the stack.</remarks>
		public virtual Key Key (int n)
		{
			if (n < 0 || n >= keyTop) {
				return null;
			}
			return keyStack [n];
		}

		/// <summary>Push a key in the stack.</summary>
		/// <remarks>
		/// Push a key in the stack.
		/// Keep the highest rank keys at the bottom.
		/// </remarks>
		public virtual void PushKey (Key key)
		{
			if (key == null) {
				ConsoleSurrogate.WriteLine ("push null key");
				return;
			}
			int i;
			for (i = keyTop; i > 0; i--) {
				if (key.rank > keyStack [i - 1].rank) {
					keyStack [i] = keyStack [i - 1];
				} else {
					break;
				}
			}
			keyStack [i] = key;
			keyTop++;
		}
	}
}
