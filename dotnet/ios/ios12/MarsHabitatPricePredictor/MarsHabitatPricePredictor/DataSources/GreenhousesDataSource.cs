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

	public string GetTitle (int index) =>
		index < Values.Length ? Values[index].ToString () : throw new IndexOutOfRangeException ($"{nameof (Values)} does not have index: {index}");

	public double GetValue (int index) =>
		index < Values.Length ? Values[index] : throw new IndexOutOfRangeException ($"{nameof (Values)} does not have index: {index}");
}
