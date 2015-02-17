using System;

namespace ListerKit
{
	public interface IListDocumentDelegate
	{
		void WasDeleted(ListDocument document);
	}
}

