using MarsHabitatPricePredictor.DataSources;

namespace MarsHabitatPricePredictor;

/// <summary>
/// Main view controller for the MarsHabitatPricer app. Uses a `UIPickerView` to gather user inputs. The model's output is the predicted price.
/// </summary>
public partial class ViewController : UIViewController, IUIPickerViewDelegate
{
	private readonly MarsHabitatPricer model = new ();

	// Data source for the picker.
	private readonly PickerDataSource pickerDataSource = new ();

	// Formatter for the output.
	private readonly NSNumberFormatter priceFormatter = new ()
	{
		NumberStyle = NSNumberFormatterStyle.Currency,
		MaximumFractionDigits = 0,
		UsesGroupingSeparator = true,
		Locale = new NSLocale ("en_US")
	};

	protected ViewController (IntPtr handle) : base (handle) { }

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();

		pickerView.Delegate = this;
		pickerView.DataSource = pickerDataSource;

		// set default values
		var features = Enum.GetValues (typeof (Feature)).Cast<Feature> ();
		foreach (var feature in features)
		{
			pickerView.Select (2, (int)feature, false);
		}

		UpdatePredictedPrice ();
	}

	/// <summary>
	/// The main logic for the app, performing the integration with Core ML.
	/// First gather the values for input to the model. Then have the model generate
	/// a prediction with those inputs. Finally, present the predicted value to the user.
	/// </summary>
	private void UpdatePredictedPrice ()
	{
		var solarPanels = pickerDataSource.GetValue (SelectedRow (Feature.SolarPanels), Feature.SolarPanels);
		var greenhouses = pickerDataSource.GetValue (SelectedRow (Feature.Greenhouses), Feature.Greenhouses);
		var size = pickerDataSource.GetValue (SelectedRow (Feature.Size), Feature.Size);

		var marsHabitatPricerOutput = model.GetPrediction (solarPanels, greenhouses, size, out NSError? error);
		if (error is not null)
			throw new Exception ($"Unexpected runtime error: {error}");

		var price = marsHabitatPricerOutput?.Price ?? throw new InvalidOperationException (nameof (marsHabitatPricerOutput));
		priceLabel.Text = priceFormatter.StringFor (NSNumber.FromDouble (price));

		int SelectedRow (Feature feature)
		{
			return (int)pickerView.SelectedRowInComponent ( (int)feature);
		}
	}

	#region IUIPickerViewDelegate

	[Export ("pickerView:didSelectRow:inComponent:")]
	public void Selected (UIPickerView pickerView, nint row, nint component)
	{
		// When values are changed, update the predicted price.
		UpdatePredictedPrice ();
	}

	[Export ("pickerView:titleForRow:forComponent:")]
	public string GetTitle (UIPickerView pickerView, nint row, nint component)
	{
		// Accessor for picker values.
		var feature = (Feature) (int)component;
		return pickerDataSource.GetTitle ( (int)row, feature);
	}

	#endregion
}

/// <summary>
/// Represents the different features used by this model. Each feature
/// of solar panels, # of greenhouses, or size) is an input value to the
/// model. So each needs an appropriate `UIPicker` as well.
/// </summary>
public enum Feature {
	SolarPanels = 0,
	Greenhouses = 1,
	Size = 2
}
