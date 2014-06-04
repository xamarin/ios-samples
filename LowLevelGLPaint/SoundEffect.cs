using System;
using Foundation;
using AudioToolbox;

namespace LowLevelGLPaint
{
	public class SoundEffect : NSObject
	{
		SystemSound sound;

		public SoundEffect (string path)
		{
			sound = SystemSound.FromFile (new NSUrl (path, false));
		}

		protected override void Dispose (bool disposing)
		{
			((IDisposable) sound).Dispose ();
			sound = null;
		}

		public void Play ()
		{
			sound.PlaySystemSound ();
		}
	}
}

