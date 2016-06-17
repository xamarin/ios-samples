using System;

namespace GrowRowTable
{
	public class GrowItem
	{
		#region Computed Properties
		public string ImageName { get; set; } = "";
		public string Title { get; set; } = "";
		public string Description { get; set; } = "";
		#endregion

		#region Constructors
		public GrowItem ()
		{
		}

		public GrowItem (string imageName, string title, string description)
		{
			// Initialize
			this.ImageName = imageName;
			this.Title = title;
			this.Description = description;
		}
		#endregion

	}
}

