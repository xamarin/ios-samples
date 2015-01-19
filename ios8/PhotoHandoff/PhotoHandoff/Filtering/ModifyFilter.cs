using System;
using Foundation;

namespace PhotoHandoff
{
	public class ModifyFilter : ImageFilter
	{
		const string IntensityKey = "kImageFilterIntensityKey";
		internal const string Key = "ModifyFilter";

		public float Intensity { get; set; }

		public ModifyFilter (bool active)
			: base(active)
		{
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

