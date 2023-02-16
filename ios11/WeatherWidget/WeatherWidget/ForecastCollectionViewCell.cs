using System;
using UIKit;

namespace WeatherWidget {
	/// <summary>
	/// A collection view cell to show a single day's weather forecast in the main app.
	/// </summary>
	public partial class ForecastCollectionViewCell : UICollectionViewCell {
		protected ForecastCollectionViewCell (IntPtr handle) : base (handle) { }

		/// The reuse identifier for this collection view cell.
		public new static string ReuseIdentifier { get; } = "ForecastCollectionViewCell";

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
