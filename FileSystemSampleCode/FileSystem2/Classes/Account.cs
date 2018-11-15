using System;
using System.Collections.Generic;

using Foundation;

namespace FileSystem
{
	// We use the Preserve attribute to ensure that all the properties of the object
	// are preserve even when the linker is ran on the assembly. The reasoning
	// for this pattern is to ensure that libraries, such as Newsoft.Json, that use
	// reflection can find properties that could be removed by the linker.
	[Preserve]
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
