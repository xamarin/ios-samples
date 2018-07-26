using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;

namespace PlacingObjects
{
	public class VirtualObjectDefinition : IEquatable<VirtualObjectDefinition>
	{
		public String ModelName { get; }
		public String DisplayName { get; }
		public Dictionary<string, float> ParticleScaleInfo { get; }

		public UIImage ThumbImage { get; protected set; }

		public VirtualObjectDefinition(string modelName, string displayName, Dictionary<string, float> particleScaleInfo = null)
		{
			this.ModelName = modelName;
			this.DisplayName = displayName;
			this.ParticleScaleInfo = particleScaleInfo == null ? new Dictionary<string, float>() : particleScaleInfo;

			this.ThumbImage = UIImage.FromBundle(this.ModelName);
		}

		public bool Equals(VirtualObjectDefinition other)
		{
			if (this.ModelName.Equals(other.ModelName)
				&& this.DisplayName.Equals(other.DisplayName))
			{
				var thisByKey = this.ParticleScaleInfo.OrderBy(kv => kv.Key);
				var otherByKey = other.ParticleScaleInfo.OrderBy(kv => kv.Key);
				return thisByKey.SequenceEqual(otherByKey);
			}
			else
			{
				return false;
			}

		}
	}
}
