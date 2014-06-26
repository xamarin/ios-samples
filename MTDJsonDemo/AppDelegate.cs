using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using MonoTouch.Dialog;

namespace MTDJsonDemo
{   
	[Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow _window;
		UINavigationController _nav;
		DialogViewController _rootVC;
		RootElement _rootElement;
		UIBarButtonItem _addButton;
		int n = 0;
        
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			_window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			_rootElement = new RootElement ("Json Example"){
                new Section ("Demo Json"){
                    (Element)JsonElement.FromFile ("sample.json"),
                    (Element)new JsonElement ("Load from url", "http://localhost/sample.json")
                },
				new Section ("Tasks Sample")
            };
     
			_rootVC = new DialogViewController (_rootElement);
			_nav = new UINavigationController (_rootVC);
            
			_addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add);
			_rootVC.NavigationItem.RightBarButtonItem = _addButton;
            
			_addButton.Clicked += (sender, e) => {
                
				++n;
                
				var task = new Task{Name = "task " + n, DueDate = DateTime.Now};
                
				var taskElement = JsonElement.FromFile ("task.json");
                
				taskElement.Caption = task.Name;
                
				var description = taskElement ["task-description"] as EntryElement;
                
				if (description != null) {
					description.Caption = task.Name;
					description.Value = task.Description;       
				}
                
				var duedate = taskElement ["task-duedate"] as DateElement;
                
				if (duedate != null) {                
					duedate.DateValue = task.DueDate;
				}
         
				_rootElement [1].Add ((Element)taskElement);
			};
            
			_window.RootViewController = _nav;
			_window.MakeKeyAndVisible ();
            
			return true;
		}
	}
}

