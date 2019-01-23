using System;

namespace XMBindingLibrary
{
	partial class XMUtilities
	{
		// NOTE:
		// We can add additional members in this file that were never in the
		// original binding.
		public nint Subtract(nint operandUn, nint operandDeux)
		{
			return Add(operandUn, -operandDeux);
		}
	}
}
