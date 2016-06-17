// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Fox
{
	[Register ("GameView")]
	partial class GameView
	{
		[Outlet]
		Fox.GameViewController Controller { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Controller != null) {
				Controller.Dispose ();
				Controller = null;
			}
		}
	}
}
