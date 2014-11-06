using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Foundation;
using SysSound.Extensions;
using AudioToolbox;

namespace SysSound {
	
	public partial class SysSoundViewController : UIViewController {
		
		//loads the SysSoundViewController.xib file and connects it to this object
		public SysSoundViewController () 
			: base ("SysSoundViewController", null) {
		}
		
		//holds the sound to play
		private SystemSound Sound;
		
		//prepares the audio
		public override void ViewDidLoad () {
			base.ViewDidLoad ();
			
			//enable audio
			AudioSession.Initialize();
			
			//load the sound
			Sound = SystemSound.FromFile("Sounds/tap.aif");
    		
		}
		
		partial void playSystemSound(NSObject sender) {
			Sound.PlaySystemSound(); 
		}
		
		partial void playAlertSound (NSObject sender) {
			Sound.PlayAlertSound();
		}

		partial void vibrate (NSObject sender) {
			SystemSound.Vibrate.PlaySystemSound();
		}
		
	}

	
}
