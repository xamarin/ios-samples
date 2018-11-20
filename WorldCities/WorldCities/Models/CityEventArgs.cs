
namespace WorldCities
{
    public class CityEventArgs : System.EventArgs
    {
        public WorldCity City { get; private set; }

        public CityEventArgs(WorldCity city)
        {
            City = city;
        }
    }
}