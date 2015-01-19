using System;

using UIKit;
using Foundation;
using ObjCRuntime;

using Common;

namespace ListerKit
{
	[Register("CheckBox")]
	public class CheckBox : UIControl
	{
		[Export("layerClass")]
		public static Class LayerClass {
			get {
				return new Class(typeof(CheckBoxLayer));
			}
		}

		CheckBoxLayer CheckBoxLayer {
			get {
				return (CheckBoxLayer)Layer;
			}
		}

		[Outlet("checked")]
		public bool Checked {
			get {
				return CheckBoxLayer.Checked;
			}
			set {
				CheckBoxLayer.Checked = value;
				UIAccessibility.PostNotification (UIAccessibilityPostNotification.LayoutChanged, null);
			}
		}

		public float StrokeFactor {
			get {
				return CheckBoxLayer.StrokeFactor;
			}
			set {
				CheckBoxLayer.StrokeFactor = value;
			}
		}

		public float InsetFactor {
			get {
				return CheckBoxLayer.InsetFactor;
			}
			set {
				CheckBoxLayer.InsetFactor = value;
			}
		}

		public float MarkInsetFactor {
			get {
				return CheckBoxLayer.MarkInsetFactor;
			}
			set {
				CheckBoxLayer.MarkInsetFactor = value;
			}
		}

		public CheckBox (IntPtr handle)
			: base(handle)
		{
		}

		public override void TintColorDidChange ()
		{
			base.TintColorDidChange ();
			CheckBoxLayer.TintColor = TintColor.CGColor;
		}
	}
}
