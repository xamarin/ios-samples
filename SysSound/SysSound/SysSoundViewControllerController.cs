using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using SysSound.Extensions;
using MonoTouch.AudioToolbox;

namespace SysSound {
	
	public partial class SysSoundViewController : UIViewController {
		
		//loads the SysSoundViewController.xib file and connects it to this object
		public SysSoundViewController () 
			: base ("SysSoundViewController", null) {
		}
		
		//holds the sound to play
		private SystemSound _Sound;
		
		//prepares the audio
		public override void ViewDidLoad () {
			base.ViewDidLoad ();
			
			//enable audio
			AudioSession.Initialize();
			
			//load the sound
			var url = NSBundle.MainBundle.URLForResource("tap", "aif");
			this._Sound = SystemSound.FromFile(url);
    		
		}
		
		partial void playSystemSound(NSObject sender) {
			this._Sound.PlaySystemSound(); 
		}
		
		partial void playAlertSound (NSObject sender) {
			this._Sound.PlayAlertSound();
		}

		partial void vibrate (NSObject sender) {
			SystemSound.Vibrate.PlaySystemSound();
		}
		
	}

	
}
