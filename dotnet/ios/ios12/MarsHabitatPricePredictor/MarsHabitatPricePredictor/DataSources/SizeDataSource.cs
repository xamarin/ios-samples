namespace MarsHabitatPricePredictor.DataSources;

/// <summary>
/// Data source for the size field on the UIPicker.
/// </summary>
public class SizeDataSource
{
	// Helper formatter to represent large nubmers in the picker
	static readonly NSNumberFormatter numberFormatter = new ()
	{
		Locale = NSLocale.CurrentLocale,
		NumberStyle = NSNumberFormatterStyle.Decimal,
		UsesGroupingSeparator = true
	};

	/// <summary>
	/// Possible values for size of the habitat.
	/// </summary>
	public double[] Values { get; } = { 750, 1000, 1500, 2000, 3000, 4000, 5000, 10000 };

	public string GetTitle (int index) =>
		index < Values.Length ? numberFormatter.StringFor (NSNumber.FromDouble (Values[index])) : throw new IndexOutOfRangeException ($"{nameof (Values)} does not have index: {index}");


	public double GetValue (int index) =>
		index < Values.Length ? Values[index] : throw new IndexOutOfRangeException ($"{nameof (Values)} does not have index: {index}");
}
