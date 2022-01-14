namespace MarsHabitatPricePredictor.DataSources;

/// <summary>
/// The common data source for the three features and their picker values. Decouples
/// the user interface and the feature specific values.
/// </summary>
public class PickerDataSource : UIPickerViewDataSource
{
	private readonly SolarPanelDataSource solarPanelsDataSource = new ();
	private readonly GreenhousesDataSource greenhousesDataSource = new ();
	private readonly SizeDataSource sizeDataSource = new ();

	/// <summary>
	/// Find the title for the given feature.
	/// </summary>
	public string GetTitle (int row, Feature feature) => feature switch
	{
		Feature.SolarPanels => this.solarPanelsDataSource.GetTitle (row),
		Feature.Greenhouses => this.greenhousesDataSource.GetTitle (row),
		Feature.Size => this.sizeDataSource.GetTitle (row),
		_ => throw new ArgumentOutOfRangeException ($"Unknown feature: {feature}"),
	};

	/// <summary>
	/// For the given feature, find the value for the given row.
	/// </summary>
	public double GetValue (int row, Feature feature) => feature switch
	{
		Feature.SolarPanels => this.solarPanelsDataSource.GetValue (row),
		Feature.Greenhouses => this.greenhousesDataSource.GetValue (row),
		Feature.Size => this.sizeDataSource.GetValue (row),
		_ => throw new ArgumentOutOfRangeException ($"Unknown feature: {feature}"),
	};

	#region UIPickerViewDataSource

	public override nint GetComponentCount (UIPickerView pickerView) => 3;

	public override nint GetRowsInComponent (UIPickerView pickerView, nint component) => (Feature) (int)component switch
	{
		Feature.SolarPanels => this.solarPanelsDataSource.Values.Length,
		Feature.Greenhouses => this.greenhousesDataSource.Values.Length,
		Feature.Size => this.sizeDataSource.Values.Length,
		_ => throw new NotImplementedException (),
	};

	#endregion
}
