using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace GrowRowTable
{
	partial class GrowRowTableCell : UITableViewCell
	{
		
		#region Computed Properties
		public UIImage Image {
			get { return CellImage.Image; }
			set { CellImage.Image = value; }
		}

		public string Title {
			get { return CellTitle.Text; }
			set { CellTitle.Text = value; }
		}

		public string Description {
			get { return CellDescription.Text; }
			set { CellDescription.Text = value; }
		}
		#endregion

		#region Constructors
		public GrowRowTableCell (IntPtr handle) : base (handle)
		{
		}
		#endregion
	}
}
