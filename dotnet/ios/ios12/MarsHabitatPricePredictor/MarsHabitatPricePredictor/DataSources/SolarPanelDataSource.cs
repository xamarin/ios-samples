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

    public string? Title(int index)
    {
        string? result = null;
        if (index < this.Values.Length)
        {
            result = this.Values[index].ToString();
        }

        return result;
    }

    public double? Value(int index)
    {
        double? result = null;
        if (index < this.Values.Length)
        {
            result = this.Values[index];
        }

        return result;
    }
}
