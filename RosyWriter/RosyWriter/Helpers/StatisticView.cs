using Foundation;
using RosyWriter.Helpers;
using System;
using UIKit;

namespace RosyWriter
{
    public partial class StatisticView : UIView
    {
        public StatisticView (IntPtr handle) : base (handle) { }

        internal void UpdateLabel(RosyWriterVideoProcessor videoProcessor)
        {
            fpsLabel.Text = String.Format("{0:F} FPS", videoProcessor.VideoFrameRate);

            var dimension = videoProcessor.VideoDimensions;
            dimensionsLabel.Text = String.Format("{0} x {1}", dimension.Width, dimension.Height);

            // Turn the integer constant into something human readable
            var type = videoProcessor.VideoType;
            char[] code = new char[4];
            for (int i = 0; i < 4; i++)
            {
                code[3 - i] = (char)(type & 0xff);
                type = type >> 8;
            }
            var typeString = new String(code);

            colorLabel.Text = typeString;
        }
    }
}