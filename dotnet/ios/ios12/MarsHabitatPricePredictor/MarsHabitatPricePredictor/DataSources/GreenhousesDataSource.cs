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

    public string Title(int index)
    {
        string result = null;
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
