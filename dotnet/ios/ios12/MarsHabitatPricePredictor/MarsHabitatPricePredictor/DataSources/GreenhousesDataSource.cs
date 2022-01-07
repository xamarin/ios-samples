namespace MarsHabitatPricePredictor.DataSources;

/// <summary>
/// Data source for the number of greenhouses.
/// </summary>
public class GreenhousesDataSource
{
    /// <summary>
    /// Possible values for greenhouses in the habitat
    /// </summary>
    public double[] Values { get; } = { 1, 2, 3, 4, 5 };

    public string? Title (int index) =>
        index < this.Values.Length ? this.Values[index].ToString () : null;

    public double? Value (int index) =>
        index < this.Values.Length ? this.Values[index] : null;
}
