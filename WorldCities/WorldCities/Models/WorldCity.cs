
namespace WorldCities {
	public class WorldCity {
		public WorldCity (string name, double latitude, double longitude)
		{
			this.Name = name;
			this.Latitude = latitude;
			this.Longitude = longitude;
		}

		public string Name { get; private set; }

		public double Latitude { get; private set; }

		public double Longitude { get; private set; }
	}
}
