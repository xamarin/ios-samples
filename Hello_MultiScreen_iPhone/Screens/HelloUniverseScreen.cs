using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Hello_MultiScreen_iPhone
{
	public partial class HelloUniverseScreen : UIViewController
	{
		//loads the HelloUniverseScreen.xib file and connects it to this object
		public HelloUniverseScreen () : base ("HelloUniverseScreen", null)
		{
			this.Title = "Universe!";
		}
	}
}
