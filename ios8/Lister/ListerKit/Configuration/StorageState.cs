using System;

namespace ListerKit
{
	public struct StorageState
	{
		public StorageType StorageOption { get; set; }
		public bool AccountDidChange { get; set; }
		public bool CloudAvailable { get; set; }
	}
}
