
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_StandardControls.Screens.iPhone.PickerView
{
	public partial class PickerView1_iPhone : UIViewController
	{
		PickerDataModel pickerDataModel;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public PickerView1_iPhone (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public PickerView1_iPhone (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public PickerView1_iPhone () : base("PickerView1_iPhone", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.Title = "Picker View";
			
			// create our simple picker model
			pickerDataModel = new PickerDataModel ();
			pickerDataModel.Items.Add ("item the first!");
			pickerDataModel.Items.Add ("item the second!");
			pickerDataModel.Items.Add ("item the third!");
			pickerDataModel.Items.Add ("fourth item!");
			
			// set it on our picker class
			this.pkrMain.Source = pickerDataModel;
			
			// wire up the value change method
			pickerDataModel.ValueChanged += (s, e) => {
				this.lblSelectedItem.Text = pickerDataModel.SelectedItem;
			};
				
			// set our initial selection on the label
			this.lblSelectedItem.Text = pickerDataModel.SelectedItem;
		}
		
		/// <summary>
		/// This is our simple picker model. it uses a list of strings as it's data and exposes
		/// a ValueChanged event when the picker changes.
		/// </summary>
		protected class PickerDataModel : UIPickerViewModel 
		{

			public event EventHandler<EventArgs> ValueChanged;
			
			/// <summary>
			/// The items to show up in the picker
			/// </summary>
			public List<string> Items
			{
				get { return items; }
				set { items = value; }
			}
			List<string> items = new List<string>();
			
			/// <summary>
			/// The current selected item
			/// </summary>
			public string SelectedItem
			{
				get { return items[selectedIndex]; }
			}
			protected int selectedIndex = 0;
			
			/// <summary>
			/// default constructor
			/// </summary>
			public PickerDataModel ()
			{
			}
		
			/// <summary>
			/// Called by the picker to determine how many rows are in a given spinner item
			/// </summary>
			public override int GetRowsInComponent (UIPickerView picker, int component)
			{
				return items.Count;
			}
			
			/// <summary>
			/// called by the picker to get the text for a particular row in a particular 
			/// spinner item
			/// </summary>
			public override string GetTitle (UIPickerView picker, int row, int component)
			{
				return items[row];
			}
			
			/// <summary>
			/// called by the picker to get the number of spinner items
			/// </summary>
			public override int GetComponentCount (UIPickerView picker)
			{
				return 1;
			}
			
			/// <summary>
			/// called when a row is selected in the spinner
			/// </summary>
			public override void Selected (UIPickerView picker, int row, int component)
			{
				selectedIndex = row;
				if (this.ValueChanged != null)
				{
					this.ValueChanged (this, new EventArgs ());
				}	
			}
		}		
	}
}

