using System;
using UIKit;

namespace MarsHabitatPricePredictor.DataSources {
	/// <summary>
	/// The common data source for the three features and their picker values. Decouples
	/// the user interface and the feature specific values.
	/// </summary>
	public class PickerDataSource : UIPickerViewDataSource {
		private readonly SolarPanelDataSource solarPanelsDataSource = new SolarPanelDataSource ();
		private readonly GreenhousesDataSource greenhousesDataSource = new GreenhousesDataSource ();
		private readonly SizeDataSource sizeDataSource = new SizeDataSource ();

		/// <summary>
		/// Find the title for the given feature.
		/// </summary>
		public string Title (int row, Feature feature)
		{
			string result = null;
			switch (feature) {
			case Feature.SolarPanels:
				result = this.solarPanelsDataSource.Title (row);
				break;
			case Feature.Greenhouses:
				result = this.greenhousesDataSource.Title (row);
				break;
			case Feature.Size:
				result = this.sizeDataSource.Title (row);
				break;
			}

			return result;
		}

		/// <summary>
		/// For the given feature, find the value for the given row.
		/// </summary>
		public double Value (int row, Feature feature)
		{
			double? result = null;

			switch (feature) {
			case Feature.SolarPanels:
				result = this.solarPanelsDataSource.Value (row);
				break;
			case Feature.Greenhouses:
				result = this.greenhousesDataSource.Value (row);
				break;
			case Feature.Size:
				result = this.sizeDataSource.Value (row);
				break;
			}

			return result.Value;
		}

		#region UIPickerViewDataSource

		public override nint GetComponentCount (UIPickerView pickerView)
		{
			return 3;
		}

		public override nint GetRowsInComponent (UIPickerView pickerView, nint component)
		{
			switch ((Feature) (int) component) {
			case Feature.SolarPanels:
				return this.solarPanelsDataSource.Values.Length;
			case Feature.Greenhouses:
				return this.greenhousesDataSource.Values.Length;
			case Feature.Size:
				return this.sizeDataSource.Values.Length;
			default:
				throw new NotImplementedException ();
			}
		}

		#endregion
	}
}
