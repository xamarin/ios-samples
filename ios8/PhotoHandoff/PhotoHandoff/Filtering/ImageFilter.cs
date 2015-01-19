using System;

using Foundation;
using UIKit;
using ObjCRuntime;

namespace PhotoHandoff
{
	public class ImageFilter : UIStateRestoring
	{
		const string ActiveKey = "kImageFilterActiveKey";

		public event EventHandler DirtyChanged;

		public bool Active { get; set; }

		bool dirty;
		public bool Dirty {
			get {
				return dirty;
			}
			set {
				dirty = value;

				var handler = DirtyChanged;
				if (handler != null)
					handler (this, EventArgs.Empty);
			}
		}

		public Type RestorationType { get; set; }

		Class objectRestorationClass;
		public override Class ObjectRestorationClass {
			get {
				objectRestorationClass = objectRestorationClass ?? new Class (RestorationType);
				return objectRestorationClass;
			}
		}

		public ImageFilter (bool active)
		{
			Active = active;
		}

		public override void EncodeRestorableState (NSCoder coder)
		{
			coder.Encode (Active, ActiveKey);
		}

		public override void DecodeRestorableState (NSCoder coder)
		{
			if (coder.ContainsKey (ActiveKey))
				Active = coder.DecodeBool (ActiveKey);
		}
	}
}

