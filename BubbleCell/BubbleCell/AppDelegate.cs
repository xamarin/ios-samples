//
// Sample shows how to use MonoTouch.Dialog to create an iPhone SMS-like 
// display of conversations
//
// Author:
//   Miguel de Icaza 
//
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BubbleCell
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		DialogViewController chat;
		UIWindow window;
		Section chatSection;
		
		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			var root = new RootElement ("Chat Sample") {
				(chatSection = new Section () {
					new ChatBubble (false, " "),
					new ChatBubble (false, "           "),
					new ChatBubble (false, "           \n           \n           \n           "),
					new ChatBubble (false, "                                                        \n                    "),
					new ChatBubble (true, "This is the text on the left, what I find fascinating about this is how many lines can fit!"),
					new ChatBubble (false, "This is some text on the right"),
					new ChatBubble (true, "Wow, you are very intense!"),
					new ChatBubble (false, "oops"),
					new ChatBubble (true, "yes"),
				})
			};
			chat = new DialogViewController (UITableViewStyle.Plain, root);
			chat.TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			window.RootViewController = chat;
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

