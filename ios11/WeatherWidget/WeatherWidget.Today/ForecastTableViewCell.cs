using Foundation;
using System;
using UIKit;

namespace WeatherWidget {
	/// <summary>
	/// A table view cell to show a single day's weather forecast in the today extension.
	/// </summary>
	public partial class ForecastTableViewCell : UITableViewCell {
		protected ForecastTableViewCell (IntPtr handle) : base (handle) { }

		public new static string ReuseIdentifier { get; } = "ForecastTableViewCell";

		// Heights for the two styles of cell display.
		public static float TodayCellHeight { get; } = 110f;

		public static float StandardCellHeight { get; } = 55f;

		public string Label {
			get => this.dateLabel.Text;
			set => this.dateLabel.Text = value;
		}

		public UIImage Image {
			get => this.forecastImageView.Image;
			set => this.forecastImageView.Image = value;
		}

		public string ForecastLabel {
			get => this.forecastLabel.Text;
			set => this.forecastLabel.Text = value;
		}
	}
}
