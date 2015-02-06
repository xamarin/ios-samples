using System;

namespace ListerKit
{
	public enum BatchChangeKind
	{
		None,
		Removed,
		Inserted,
		Toggled,
		UpdatedText,
		Multiple
	}
}

