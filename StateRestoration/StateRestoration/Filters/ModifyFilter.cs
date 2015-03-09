using System;
using Foundation;

namespace StateRestoration
{
	public class ModifyFilter : ImageFilter
	{
		const string IntensityKey = "kImageFilterIntensityKey";

		public float Intensity {
			get {
				return Value;
			}
			set {
				Value = value;
			}
		}

		public ModifyFilter (bool useDefaultState)
			: base (useDefaultState)
		{
			if (useDefaultState)
				Intensity = 0.5f;
		}

		public override void EncodeRestorableState (NSCoder coder)
		{
			base.EncodeRestorableState (coder);
			coder.Encode (Intensity, IntensityKey);
		}

		public override void DecodeRestorableState (NSCoder coder)
		{
			base.DecodeRestorableState (coder);
			if (coder.ContainsKey (IntensityKey))
				Intensity = coder.DecodeFloat (IntensityKey);
		}
	}
}
