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