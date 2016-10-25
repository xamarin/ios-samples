// class translated from Java
// Credit goes to Charles Hayden http://www.chayden.net/eliza/Eliza.html

namespace ElizaCore
{
	/// <summary>Eliza memory class</summary>
	public class Mem
	{
		/// <summary>The memory size</summary>
		internal static readonly int memMax = 20;

		/// <summary>The memory</summary>
		internal string[] memory = new string[memMax];

		/// <summary>The memory top</summary>
		internal int memTop = 0;

		public virtual void Save(string str)
		{
			if (memTop < memMax)
			{
				memory[memTop++] = str;
			}
		}

		public virtual string Get()
		{
			if (memTop == 0)
			{
				return null;
			}
			string m = memory[0];
			for (int i = 0; i < memTop - 1; i++)
			{
				memory[i] = memory[i + 1];
			}
			memTop--;
			return m;
		}
	}
}
