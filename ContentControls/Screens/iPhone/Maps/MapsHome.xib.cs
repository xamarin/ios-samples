
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_ContentControls.Screens.iPhone.Maps
{
	public partial class MapsHome : UIViewController
	{
		BasicMapScreen basicMaps = null;
		AnnotatedMapScreen mapWithAnnotations = null;
		MapWithOverlayScreen mapWithOverlay = null;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public MapsHome (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public MapsHome (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public MapsHome () : base("MapsHome", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.Title = "Maps";
			
			this.btnBasicMap.TouchUpInside += (s, e) => {
				if(basicMaps == null)
					basicMaps = new BasicMapScreen();
				this.NavigationController.PushViewController(basicMaps, true);	
			};
			
			this.btnMapWithAnnotations.TouchUpInside += (s, e) => {
				if(mapWithAnnotations == null)
					mapWithAnnotations = new AnnotatedMapScreen();
				this.NavigationController.PushViewController(mapWithAnnotations, true);	
			};
			
			this.btnMapWithOverlay.TouchUpInside += (s, e) => {
				if(mapWithOverlay == null)
					mapWithOverlay = new MapWithOverlayScreen();
				this.NavigationController.PushViewController(mapWithOverlay, true);	
			};
		}
	}
}

