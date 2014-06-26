using System;

using Foundation;
using CoreMotion;
using UIKit;

using MotionGraphs;

namespace MotionGraphs
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		CMMotionManager motion_manager;

		public CMMotionManager SharedManager {
			get {
				if (motion_manager == null) 
					motion_manager = new CMMotionManager ();
				return motion_manager;
			}
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			UITabBarController tabBarController = new UITabBarController ();
			tabBarController.SetViewControllers (new UIViewController[] {
				new GraphViewController ("Accelerometer", MotionDataType.AccelerometerData), 
				new GraphViewController ("Gyro", MotionDataType.GyroData), 
				new GraphViewController ("DeviceMotion", MotionDataType.DeviceMotion)
			}, true);

			window.RootViewController = tabBarController;
			window.MakeKeyAndVisible ();

			return true;
		}
	}
}