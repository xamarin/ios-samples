using System;
using System.Collections.Generic;

namespace FileSystem
{
	public class Account
	{
		#region Computed Properties
		public string Email { get; set; }
		public bool Active { get; set; }
		public DateTime CreatedDate { get; set; }
		public List<string> Roles { get; set; }
		#endregion

		#region Constructors
		public Account() {

		}
		#endregion
	}
}

