using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;

namespace tvCollection
{
	public class CityViewDatasource : UICollectionViewDataSource
	{
		#region Application Access
		public static AppDelegate App {
			get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
		}
		#endregion

		#region Static Constants
		public static NSString CardCellId = new NSString ("CityCell");
		#endregion

		#region Computed Properties
		public List<CityInfo> Cities { get; set; } = new List<CityInfo>();
		public CityCollectionView ViewController { get; set; }
		#endregion

		#region Constructors
		public CityViewDatasource (CityCollectionView controller)
		{
			// Initialize
			this.ViewController = controller;
			PopulateCities ();
		}
		#endregion

		#region Public Methods
		public void PopulateCities() {

			// Clear existing cities
			Cities.Clear();

			// Add new cities
			Cities.Add(new CityInfo("City01.jpg", "Houses by Water", false));
			Cities.Add(new CityInfo("City02.jpg", "Turning Circle", true));
			Cities.Add(new CityInfo("City03.jpg", "Skyline at Night", true));
			Cities.Add(new CityInfo("City04.jpg", "Golden Gate Bridge", true));
			Cities.Add(new CityInfo("City05.jpg", "Roads by Night", true));
			Cities.Add(new CityInfo("City06.jpg", "Church Domes", true));
			Cities.Add(new CityInfo("City07.jpg", "Mountain Lights", true));
			Cities.Add(new CityInfo("City08.jpg", "City Scene", false));
			Cities.Add(new CityInfo("City09.jpg", "House in Winter", true));
			Cities.Add(new CityInfo("City10.jpg", "By the Lake", true));
			Cities.Add(new CityInfo("City11.jpg", "At the Dome", true));
			Cities.Add(new CityInfo("City12.jpg", "Cityscape", true));
			Cities.Add(new CityInfo("City13.jpg", "Model City", true));
			Cities.Add(new CityInfo("City14.jpg", "Taxi, Taxi!", true));
			Cities.Add(new CityInfo("City15.jpg", "On the Sidewalk", true));
			Cities.Add(new CityInfo("City16.jpg", "Midnight Walk", true));
			Cities.Add(new CityInfo("City17.jpg", "Lunchtime Cafe", true));
			Cities.Add(new CityInfo("City18.jpg", "Coffee Shop", true));
			Cities.Add(new CityInfo("City19.jpg", "Rustic Tavern", true));
		}
		#endregion

		#region Override Methods
		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return Cities.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cityCell = (CityCollectionViewCell)collectionView.DequeueReusableCell (CardCellId, indexPath);
			var city = Cities [indexPath.Row];

			// Initialize city
			cityCell.City = city;

			return cityCell;
		}
		#endregion
	}
}

