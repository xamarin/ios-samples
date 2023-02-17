
namespace DispatchSourceExamples.Models {
	public class DispatchSourceItem {
		public string Title { get; set; }

		public string Subtitle { get; set; }

		public DispatchSourceType Type { get; set; }
	}

	public enum DispatchSourceType {
		Timer,
		Vnode,
		MemoryPressure,
		ReadMonitor,
		WriteMonitor
	}
}
