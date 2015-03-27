using System;
using InfColorPicker;
using UIKit;

namespace InfColorPickerSample
{
	public class ColorSelectedDelegate:InfColorPickerControllerDelegate
	{
		private UIViewController _parent;
		public ColorSelectedDelegate (UIViewController parent)
		{
			_parent = parent;
		}

		public override void ColorPickerControllerDidFinish (InfColorPickerController controller)
		{
			_parent.View.BackgroundColor = controller.ResultColor;
			_parent.DismissViewController (false, null);
		}
	}
}

