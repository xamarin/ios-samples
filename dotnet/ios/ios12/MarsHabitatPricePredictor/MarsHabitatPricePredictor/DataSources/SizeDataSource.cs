namespace MarsHabitatPricePredictor.DataSources;

/// <summary>
/// Data source for the size field on the UIPicker.
/// </summary>
public class SizeDataSource
{
    // Helper formatter to represent large nubmers in the picker
    private static NSNumberFormatter numberFormatter = new NSNumberFormatter
    {
        Locale = NSLocale.CurrentLocale,
        NumberStyle = NSNumberFormatterStyle.Decimal,
        UsesGroupingSeparator = true
    };

    /// <summary>
    /// Possible values for size of the habitat.
    /// </summary>
    public double[] Values { get; } = { 750, 1000, 1500, 2000, 3000, 4000, 5000, 10000 };

    public string? Title(int index)
    {
        string? result = null;
        if (index < this.Values.Length)
        {
            result = numberFormatter.StringFor(NSNumber.FromDouble(this.Values[index]));
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
