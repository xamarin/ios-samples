using System;
using CoreAnimation;
using UIKit;
using CoreGraphics;

namespace CustomVision
{
    public class BubbleLayer : CALayer
    {
        // MARK: Public Properties

        string _string;
        public string @String
        {
            get { return _string; }
            set
            {
                _string = value;
                if (String.IsNullOrEmpty(_string))
                {
                    Opacity = 0.0f;
                }
                else
                {
                    Opacity = 1.0f;
                    Console.WriteLine("BubbleLayer set: " + _string);
                    layerLabel.String = _string;
                }
				SetNeedsLayout();
            }
        }

        public UIFont font = UIFont.FromName("Helvetica-Bold", 24.0f);
        public UIFont Font
        {
            set {
                font = value;
                layerLabel.SetFont(font.Name);
                layerLabel.FontSize = font.PointSize;
            }
        }

        UIColor textColor = UIColor.White;
        public UIColor TextColor {
            set {
                textColor = value;
                layerLabel.ForegroundColor = textColor.CGColor;
            }
        }

        float paddingHorizontal = 25;
        public float PaddingHorizontal {
            set {
                paddingHorizontal = value;
                SetNeedsLayout();
            }
        }

		float paddingVertical = 25;
		public float PaddingVertical
		{
			set
			{
				paddingVertical = value;
				SetNeedsLayout();
			}
		}

        float maxWidth = 300;
        public float MaxWidth {
            set {
                maxWidth = value;
                SetNeedsLayout();
            }
        }

        BubbleLayerLabel layerLabel = new BubbleLayerLabel();


        public BubbleLayer(string @string) : base()
        {
            _string = @string;

            BackgroundColor = new UIColor(1, 0, 217 / 255, 1).CGColor;
            BorderColor = UIColor.White.CGColor;
            BorderWidth = 3.5f;

            ContentsScale = UIScreen.MainScreen.Scale;
            AllowsEdgeAntialiasing = true;

            layerLabel.String = this.String;
            layerLabel.SetFont(font.Name);
            layerLabel.FontSize = font.PointSize;
            layerLabel.ForegroundColor = textColor.CGColor;
			layerLabel.AlignmentMode = CATextLayer.AlignmentCenter; 
            layerLabel.ContentsScale = UIScreen.MainScreen.Scale;
            layerLabel.AllowsFontSubpixelQuantization = true;
            layerLabel.Wrapped = true;
            layerLabel.UpdatePreferredSize(maxWidth - paddingHorizontal * 2);
            layerLabel.Frame = new CGRect(
                new CGPoint(paddingHorizontal, paddingVertical),
                layerLabel.PreferredFrameSize());
            AddSublayer(layerLabel);

            SetNeedsLayout();
        }

        public override void LayoutSublayers()
        {
            layerLabel.UpdatePreferredSize(maxWidth - paddingHorizontal / 2);

            var preferredSize = PreferredFrameSize();
            var diffSize = new CGSize(Frame.Size.Width - preferredSize.Width, Frame.Size.Height - preferredSize.Height);
            Frame = new CGRect(new CGPoint(Frame.X + diffSize.Width / 2, Frame.Y + diffSize.Height / 2), preferredSize);
            CornerRadius = Frame.Height / 2;

            layerLabel.Frame = new CGRect(0, paddingVertical, Frame.Width, Frame.Height);
        }

        public override CGSize PreferredFrameSize()
        {
            var layerLabelSize = layerLabel.PreferredFrameSize();
            return new CGSize(layerLabelSize.Width + paddingHorizontal * 2,
                              layerLabelSize.Height + paddingVertical * 2);
        }
    }
}
