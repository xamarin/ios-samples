using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using MonoTouch.Dialog;

namespace MTDWalkthrough
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
			
			_rootElement = new RootElement ("To Do List"){
				new Section ()
			};
     
			_rootVC = new DialogViewController (_rootElement);
			_nav = new UINavigationController (_rootVC);
            
			_addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add);
			_rootVC.NavigationItem.RightBarButtonItem = _addButton;
            
			_addButton.Clicked += (sender, e) => {
                
				++n;
                
				var task = new Task{Name = "task " + n, DueDate = DateTime.Now};

				var element = new EntryElement (task.Name, "Enter task description", task.Description);

				var dateElement = new FutureDateElement ("Due Date", task.DueDate);
                
				var taskElement = (Element)new RootElement (task.Name){
                    new Section () { 
						element
					},
                    new Section () { 
						dateElement
                    },
					new Section ("Demo Retrieving Element Value") {
						new StringElement ("Output Task Description", 
							delegate { Console.WriteLine (element.Value); })
					}
                };
				_rootElement [0].Add (taskElement);
			};
     
			_window.RootViewController = _nav;
			_window.MakeKeyAndVisible ();

			return true;
		}
     	
	}

	public class FutureDateElement : DateElement
	{
		public FutureDateElement(string caption, DateTime date) : base(caption,date)
		{

		}
		public override UIDatePicker CreatePicker ()
		{
			UIDatePicker futureDatePicker = base.CreatePicker ();
			futureDatePicker.BackgroundColor = UIColor.White;
			futureDatePicker.MinimumDate = DateTime.Today;
			return futureDatePicker;
		}

	}
}

