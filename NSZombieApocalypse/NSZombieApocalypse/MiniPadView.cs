using System;
using CoreGraphics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UIKit;
using AudioToolbox;

using CoreFoundation;
using AudioUnit;
using Foundation;
using MonoTouch.Dialog;
using AVFoundation;
using System.Linq;
using System.Threading.Tasks;

namespace NSZombieApocalypse
{
	public class MiniPadView : UIControl
	{
		AVAudioPlayer newZombieSound, removeZombieSound;
		List <WalkingDead> zombies;

		public MiniPadView (CGRect frame): base(frame)
		{
			var image = UIImage.FromBundle ("iPadImage.png");
			Frame = new CGRect (Frame.Location, image.Size);

			ClipsToBounds = false;
			var imageView = new UIImageView (image);
			AddSubview (imageView);

			zombies = new List <WalkingDead> ();

			var audioNewZombie = NSUrl.FromFilename ("NewZombie.mp3");
			var audioRemoveZombie = NSUrl.FromFilename ("RemoveZombie.mp3");
			newZombieSound = AVAudioPlayer.FromUrl (audioNewZombie);
			removeZombieSound = AVAudioPlayer.FromUrl (audioRemoveZombie);

		}
		public void PauseZombies ()
		{
			foreach (var dead in zombies)
				dead.DeAnimate ();
		}

		public void UnpauseZombies ()
		{
			foreach (var dead in zombies)
				dead.Animate ();
		}

		public void WalkingDeadDidDisassemble (WalkingDead walkingDead)
		{
			zombies.Remove (walkingDead);
			walkingDead.RemoveFromSuperview ();
		}

		public void AddZombie ()
		{
			float chrome = 50;
			var frame = new CGRect (chrome, Frame.Size.Height - 160 - chrome, 80, 200);
			var dead = new WalkingDead (frame);
			dead.WalkingDeadDidDisassemble += WalkingDeadDidDisassemble;
			zombies.Add (dead);
			AddSubview (dead);

			dead.Animate ();

			UIAccessibility.PostNotification (LayoutChangedNotification, null);

			newZombieSound.Play ();
		}

		public void RemoveZombie ()
		{ 
			WalkingDead zombie = zombies.Last ();

			if (zombie != null) { 
				zombies.Remove (zombie);
				zombie.Disassemble ();
			}

			UIAccessibility.PostNotification (LayoutChangedNotification, null);
			removeZombieSound.Play ();
		}

		public int ZombieCount {
			get {
				return zombies.Count;
			}
		}
	}
}

