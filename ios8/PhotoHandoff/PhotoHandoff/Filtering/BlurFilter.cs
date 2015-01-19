using System;
using Foundation;

namespace PhotoHandoff
{
	public class BlurFilter : ImageFilter
	{
		const string BlurRadiusKey = "kImageFilterBlurRadiusKey";
		internal const string Key = "BlurFilter";

		public float BlurRadius { get; set; }

		public BlurFilter (bool active)
			: base(active)
		{
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