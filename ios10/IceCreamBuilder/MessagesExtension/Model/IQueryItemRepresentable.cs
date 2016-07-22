using Foundation;

namespace MessagesExtension {
	public interface IQueryItemRepresentable {
		NSUrlQueryItem QueryItem { get; }

		string QueryItemKey { get; }
	}
}

