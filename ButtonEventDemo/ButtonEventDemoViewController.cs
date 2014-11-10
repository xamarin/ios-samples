using UIKit;
using CoreGraphics;
using System;
using Foundation;

namespace ButtonEventDemo
{
    public partial class ButtonEventDemoViewController : UIViewController
    {
        public ButtonEventDemoViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
        {
        }
     
        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning ();
         
            // Release any cached data, images, etc that aren't in use.
        }
     
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
         
            //Here we register for the TouchUpInside event using an outlet to
            //the UIButton created in Interface Builder. Also see the target-action
            //approach for accomplishing the same thing below.
            aButton.TouchUpInside += (o,s) => {
                Console.WriteLine ("button touched using a TouchUpInside event"); 
			}; 
            
            //You could also use a C# 2.0 style anonymous function
//            aButton.TouchUpInside += delegate {
//                Console.WriteLine ("button touched");         
//            };
            
        }
     
        public override void ViewDidUnload ()
        {
            base.ViewDidUnload ();
           
            aButton = null;
        }
     
        public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
        {
            // Return true for supported orientations
            return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
        }
        
        //This is an action method connected to the TouchUpInside event
        //of a UIButton. The action is connected via Interface Builder
        //The same thing can be accomplished with a .NET event registered
        //to the UIButton in code, as we do in the ViewDidLoad method above.

        partial void HandleButtonTouch (NSObject sender)
        {
			Console.WriteLine ("button touched using the action method");
        }
    }
}
