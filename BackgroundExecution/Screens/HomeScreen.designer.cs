using System;
using UIKit;
using Foundation;

namespace BackgroundExecution
{
	[Register("HomeScreen")]
	public partial class HomeScreen
	{
		[Outlet]
		UIButton BtnStartLongRunningTask { get; set; }
	}
}

