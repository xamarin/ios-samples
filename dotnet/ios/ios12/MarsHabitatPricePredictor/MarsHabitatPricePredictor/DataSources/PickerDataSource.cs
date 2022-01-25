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
		Feature.SolarPanels => solarPanelsDataSource.GetTitle (row),
		Feature.Greenhouses => greenhousesDataSource.GetTitle (row),
		Feature.Size => sizeDataSource.GetTitle (row),
		_ => throw new ArgumentOutOfRangeException ($"Unknown feature: {feature}"),
	};

	/// <summary>
	/// For the given feature, find the value for the given row.
	/// </summary>
	public double GetValue (int row, Feature feature) => feature switch
	{
		Feature.SolarPanels => solarPanelsDataSource.GetValue (row),
		Feature.Greenhouses => greenhousesDataSource.GetValue (row),
		Feature.Size => sizeDataSource.GetValue (row),
		_ => throw new ArgumentOutOfRangeException ($"Unknown feature: {feature}"),
	};

	#region UIPickerViewDataSource

	public override nint GetComponentCount (UIPickerView pickerView) => 3;

	public override nint GetRowsInComponent (UIPickerView pickerView, nint component) => (Feature) (int)component switch
	{
		Feature.SolarPanels => solarPanelsDataSource.Values.Length,
		Feature.Greenhouses => greenhousesDataSource.Values.Length,
		Feature.Size => sizeDataSource.Values.Length,
		_ => throw new NotImplementedException (),
	};

	#endregion
}
