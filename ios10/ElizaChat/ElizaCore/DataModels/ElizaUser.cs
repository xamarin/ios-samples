using System;
using Foundation;

namespace ElizaCore {
	public class ElizaUser : NSObject {
		#region Computed Properties
		public string ScreenName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime LastMessagedOn { get; set; }
		#endregion

		#region Constructors
		public ElizaUser (string screenName, string firstName, string lastName)
		{
			// Initialize
			this.ScreenName = screenName;
			this.FirstName = firstName;
			this.LastName = lastName;
			this.LastMessagedOn = DateTime.Now;

		}
		#endregion
	}
}
