using System;
using Foundation;
using CoreImage;
using UIKit;
using ObjCRuntime;

namespace StateRestoration
{
	public class ImageFilter : UIStateRestoring
	{
		const string ActiveKey = "kImageFilterActiveKey";

		public bool Active { get; set; }

		public bool Dirty { get; set; }

		public float Value { get; set; }

		Class restorationClass;

		public override Class ObjectRestorationClass {
			get {
				return restorationClass = restorationClass ?? new Class (RestorationType);
			}
		}

		public Type RestorationType { get; set; }

		public ImageFilter (bool useDefaultState)
		{
			Dirty = true;
			Active = useDefaultState;
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

