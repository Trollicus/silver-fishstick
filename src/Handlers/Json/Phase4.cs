using System.Text.Json.Serialization;

namespace LinkvertiseBypass.Handlers.Json;

public class Phase4
{
	public class Data
	{
		[JsonPropertyName("target")]
		public string? Target { get; set; }

		[JsonPropertyName("paste")]
		public string? Paste { get; set; }
	}

	public class Root
	{
		[JsonPropertyName("data")]
		public Data? Data { get; set; }

	}


}