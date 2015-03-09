using System;
using Foundation;

namespace StateRestoration
{
	public class BlurFilter : ImageFilter
	{
		const string BlurRadiusKey = "kImageFilterBlurRadiusKey";

		public float BlurRadius {
			get {
				return Value;
			}
			set {
				Value = value;
			}
		}

		public BlurFilter (bool useDefaultState)
			: base (useDefaultState)
		{
			if (useDefaultState)
				BlurRadius = 0.5f;
		}

		public override void EncodeRestorableState (NSCoder coder)
		{
			base.EncodeRestorableState (coder);
			coder.Encode (BlurRadius, BlurRadiusKey);
		}

		public override void DecodeRestorableState (NSCoder coder)
		{
			base.DecodeRestorableState (coder);
			if (coder.ContainsKey (BlurRadiusKey))
				BlurRadius = coder.DecodeFloat (BlurRadiusKey);
		}
	}
}

