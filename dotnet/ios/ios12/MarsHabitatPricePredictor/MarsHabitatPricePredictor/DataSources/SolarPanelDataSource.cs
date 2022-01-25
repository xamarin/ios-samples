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

	public string GetTitle (int index) =>
		index < Values.Length ? Values[index].ToString () : throw new IndexOutOfRangeException ($"{nameof (Values)} does not have index: {index}");

	public double GetValue (int index) =>
		index < Values.Length ? Values[index] : throw new IndexOutOfRangeException ($"{nameof (Values)} does not have index: {index}");
}
