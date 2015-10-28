using System;

namespace tvCollection
{
	public class CityInfo
	{
		#region Computed Properties
		public string ImageFilename { get; set; }
		public string Title { get; set; }
		public bool CanSelect{ get; set; }
		#endregion

		#region Constructors
		public CityInfo (string filename, string title, bool canSelect)
		{
			// Initialize
			this.ImageFilename = filename;
			this.Title = title;
			this.CanSelect = canSelect;
		}
		#endregion
	}
}

