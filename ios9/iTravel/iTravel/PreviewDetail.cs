using Newtonsoft.Json;

namespace iTravel {
	public class PreviewDetail {
		[JsonProperty ("picture")]
		public string Picture { get; set; }

		[JsonProperty ("caption")]
		public string Caption { get; set; }
	}
}

