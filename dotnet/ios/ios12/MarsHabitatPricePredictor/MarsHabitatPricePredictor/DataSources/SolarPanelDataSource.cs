namespace MarsHabitatPricePredictor.DataSources;

/// <summary>
/// Data source for the number of solar panels in the habitat.
/// </summary>
public class SolarPanelDataSource
{
    /// <summary>
    /// Possible values for solar panels in the habitat
    /// </summary>
    public double[] Values { get; } = { 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5 };

    public string? Title (int index) =>
        index < this.Values.Length ? this.Values[index].ToString () : null;

    public double? Value (int index) =>
        index < this.Values.Length ? this.Values[index] : null;
}
