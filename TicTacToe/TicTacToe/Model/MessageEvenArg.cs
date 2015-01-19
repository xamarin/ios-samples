using System;

namespace TicTacToe
{
	public class MessageEvenArg : EventArgs
	{
		public nint[] Indexes { get; set; }

		public MessageEvenArg (params nint[] indexes)
		{
			Indexes = indexes;
		}

		public MessageEvenArg ()
		{
		}
	}
}

