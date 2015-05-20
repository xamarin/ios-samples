using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using HomeKit;

namespace HomeKitIntro
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets the window.
		/// </summary>
		/// <value>The window.</value>
		public override UIWindow Window {get; set;}

		/// <summary>
		/// Gets or sets the home manager.
		/// </summary>
		/// <value>The home manager.</value>
		public HMHomeManager HomeManager { get; set; }


		#endregion

		#region Override Methods
		/// <summary>
		/// Finisheds the launching.
		/// </summary>
		/// <param name="application">Application.</param>
		public override void FinishedLaunching (UIApplication application)
		{
			// Attach to the Home Manager
			HomeManager = new HMHomeManager ();
			Console.WriteLine ("{0} Home(s) defined in the Home Manager", HomeManager.Homes.Count());

			// Wire-up Home Manager Events
			HomeManager.DidAddHome += (sender, e) => {
				Console.WriteLine("Manager Added Home: {0}",e.Home);
			};

			HomeManager.DidRemoveHome += (sender, e) => {
				Console.WriteLine("Manager Removed Home: {0}",e.Home);
			};
			HomeManager.DidUpdateHomes += (sender, e) => {
				Console.WriteLine("Manager Updated Homes");
			};
			HomeManager.DidUpdatePrimaryHome += (sender, e) => {
				Console.WriteLine("Manager Updated Primary Home");
			};

		}

		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation (UIApplication application)
		{
		}
		
		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
		}
		
		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
		}
		
		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}
		#endregion

		#region Events
		/// <summary>
		/// Update GUI delegate.
		/// </summary>
		public delegate void UpdateGUIDelegate();
		public event UpdateGUIDelegate UpdateGUI;

		/// <summary>
		/// Raises the update GUI event
		/// </summary>
		internal void RaiseUpdateGUI() {
			// Inform caller
			if (this.UpdateGUI != null)
				this.UpdateGUI ();
		}
		#endregion

	}
}

