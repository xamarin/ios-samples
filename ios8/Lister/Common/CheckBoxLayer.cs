using System;
using System.Linq;
using System.Drawing;

using CoreAnimation;
using Foundation;
using CoreGraphics;

namespace Common
{
	public class CheckBoxLayer : CALayer
	{
		static readonly string[] keys = new string[] {
			"tintColor",
			"checked",
			"strokeFactor",
			"insetFactor",
			"markInsetFactor"
		};

		static readonly nfloat[] components = new nfloat[] {
			0.5f, 0.5f, 0.5f
		};

		[Outlet("strokeFactor")]
		public float StrokeFactor { get; set; }

		[Outlet("insetFactor")]
		public float InsetFactor { get; set; }

		[Outlet("markInsetFactor")]
		public float MarkInsetFactor { get; set; }

		CGColor tintColor;
		[Outlet("tintColor")]
		public CGColor TintColor {
			get {
				return tintColor;
			}
			set {
				tintColor = value;
				SetNeedsDisplay ();
			}
		}

		bool isChecked;
		[Outlet("checked")]
		public bool Checked {
			get {
				return isChecked;
			}
			set {
				isChecked = value;
				SetNeedsDisplay ();
			}
		}

		public CheckBoxLayer ()
		{
			TintColor = new CGColor (CGColorSpace.CreateDeviceRGB (), components);

			StrokeFactor = 0.07f;
			InsetFactor = 0.17f;
			MarkInsetFactor = 0.34f;
		}

		public override void DrawInContext (CGContext context)
		{
			base.DrawInContext (context);
			nfloat size = NMath.Min (Bounds.Size.Width, Bounds.Size.Height);

			CGAffineTransform transform = AffineTransform;

			nfloat xTranslate = 0;
			nfloat yTranslate = 0;

			if (Bounds.Size.Width < Bounds.Size.Height)
				yTranslate = (Bounds.Size.Height - size) / 2f;
			else
				xTranslate = (Bounds.Size.Width - size) / 2f;

			transform.Translate (xTranslate, yTranslate);

			nfloat strokeWidth = StrokeFactor * size;
			nfloat checkBoxInset = InsetFactor * size;

			// Create the outer border for the check box.
			nfloat outerDimension = size - 2 * checkBoxInset;
			var checkBoxRect = new CGRect(checkBoxInset, checkBoxInset, outerDimension, outerDimension);
			checkBoxRect = transform.TransformRect (checkBoxRect);

			// Make the desired width of the outer box.
			context.SetLineWidth (strokeWidth);

			// Set the tint color of the outer box.
			context.SetStrokeColor (TintColor);

			// Draw the outer box.
			context.StrokeRect (checkBoxRect);

			// Draw the inner box if it's checked.
			if (Checked) {
				nfloat markInset = MarkInsetFactor * size;

				nfloat markDimension = size - 2 * markInset;
				var markRect = new CGRect(markInset, markInset, markDimension, markDimension);
				markRect = transform.TransformRect (markRect);

				context.SetFillColor (TintColor);
				context.FillRect (markRect);
			}
		}

		[Export ("needsDisplayForKey:")]
		public new static bool NeedsDisplayForKey(string key)
		{
			if (keys.Contains (key))
				return true;
			else
				return CALayer.NeedsDisplayForKey (key);
		}
	}
}
