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
    public string GetTitle (int row, Feature feature)
    {
        string? result = null;
        switch (feature)
        {
            case Feature.SolarPanels:
                result = this.solarPanelsDataSource.Title (row);
                break;
            case Feature.Greenhouses:
                result = this.greenhousesDataSource.Title (row);
                break;
            case Feature.Size:
                result = this.sizeDataSource.Title (row);
                break;
            default:
                throw new ArgumentOutOfRangeException ($"Unknown feature: {feature}");
        }

        return result;
    }

    /// <summary>
    /// For the given feature, find the value for the given row.
    /// </summary>
    public double GetValue (int row, Feature feature)
    {
        double? result = null;

        switch (feature)
        {
            case Feature.SolarPanels:
                result = this.solarPanelsDataSource.Value (row);
                break;
            case Feature.Greenhouses:
                result = this.greenhousesDataSource.Value (row);
                break;
            case Feature.Size:
                result = this.sizeDataSource.Value (row);
                break;
            default:
                throw new ArgumentOutOfRangeException ($"Unknown feature: {feature}");
        }

        return result.Value;
    }

    #region UIPickerViewDataSource

    public override nint GetComponentCount (UIPickerView pickerView)
    {
        return 3;
    }

    public override nint GetRowsInComponent (UIPickerView pickerView, nint component) => (Feature) (int)component switch
    {
        Feature.SolarPanels => this.solarPanelsDataSource.Values.Length,
        Feature.Greenhouses => this.greenhousesDataSource.Values.Length,
        Feature.Size => this.sizeDataSource.Values.Length,
        _ => throw new NotImplementedException (),
    };

    #endregion
}
