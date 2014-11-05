using System;
using Foundation;

namespace TicTacToe
{
	[Serializable]
	public class TTTMessage
	{
		const string EncodingKeyText = "text";
		const string EncodingKeyIcon = "icon";
		public string Text;
		public TTTProfileIcon Icon;

		public TTTMessage ()
		{

		}
	}
}

