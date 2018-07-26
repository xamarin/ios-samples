using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using ImageIO;

using CGImageProperties = ImageIO.CGImageProperties;

namespace SamplePhotoApp
{
	public class AnimatedImage
	{
		private CGImageSource imageSource;
		private double[] delays;

		public nint FrameCount { get; set; }
		public double Duration { get; set; }
		public nint LoopCount { get; set; }
		public CGSize Size { get; set; }

		public AnimatedImage (NSUrl url)
			: this (CGImageSource.FromUrl (url, null))
		{
		}

		public AnimatedImage (NSData data)
			: this (CGImageSource.FromData (data, null))
		{
		}

		public AnimatedImage (CGImageSource source)
		{
			imageSource = source;
			FrameCount = imageSource.ImageCount;

			var imageProperties = source.CopyProperties ((CGImageOptions)null);
			if (imageProperties != null)
			{
				LoopCount = LoopCountForProperties (imageProperties);
			}
			else
			{
				// The default loop count for a GIF with no loop count specified is 1.
				// Infinite loops are indicated by an explicit value of 0 for this property.
				LoopCount = 1;
			}

			var firstImage = source.CreateImage (0, null);
			if (firstImage != null)
				Size = new CGSize (firstImage.Width, firstImage.Height);
			else
				Size = CGSize.Empty;

			var delayTimes = Enumerable.Repeat (1.0 / 30.0, (int)FrameCount).ToArray ();
			var totalDuration = 0.0;
			for (var index = 0; index < FrameCount; index++)
			{
				var properties = source.CopyProperties ((CGImageOptions)null, index);
				if (properties != null)
				{
					var time = FrameDelayForProperties (properties);
					if (time != null)
					{
						delayTimes[index] = time.Value;
					}
				}
				totalDuration += delayTimes[index];
			}
			Duration = totalDuration;
			delays = delayTimes;
		}

		static double? FrameDelayForProperties (NSDictionary properties)
		{
			// Read the delay time for a GIF.
			var gifDictionary = properties[CGImageProperties.GIFDictionary] as NSDictionary;
			if (gifDictionary == null)
			{
				return null;
			}

			var delay = (gifDictionary[CGImageProperties.GIFUnclampedDelayTime] as NSNumber)?.DoubleValue;
			if (delay != null & delay > 0.0)
			{
				return delay;
			}

			delay = (gifDictionary[CGImageProperties.GIFDelayTime] as NSNumber)?.DoubleValue;
			if (delay != null & delay > 0.0)
			{
				return delay;
			}

			return null;
		}

		static nint LoopCountForProperties (NSDictionary properties)
		{
			var gifDictionary = properties[CGImageProperties.GIFDictionary] as NSDictionary;
			if (gifDictionary != null)
			{
				var loopCount = (gifDictionary[CGImageProperties.GIFLoopCount] as NSNumber)?.NIntValue;
				if (loopCount != null)
				{
					return loopCount.Value;
				}
			}

			// A single playthrough is the default if loop count metadata is missing.
			return 1;
		}

		public CGImage ImageAtIndex (int index)
		{
			if (index < FrameCount)
			{
				return imageSource.CreateImage (index, (CGImageOptions)null);
			}
			else
			{
				return null;
			}
		}

		public double DelayAtIndex (int index)
		{
			return delays[index];
		}
	}
}
