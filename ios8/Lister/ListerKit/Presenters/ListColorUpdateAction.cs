using System;

namespace ListerKit
{
	public enum ListColorUpdateAction
	{
		// TODO: Remane – remove *Delegate* from name
		DontSendDelegateChangeLayoutCalls,
		SendDelegateChangeLayoutCallsForInitialLayout,
		SendDelegateChangeLayoutCallsForNonInitialLayout
	}
}

