using System.Text.Json.Serialization;

namespace LinkvertiseBypass.Handlers.Json;

public class Phase3
{
    public class Data
    {
        
        [JsonPropertyName("tokens")]
        public Tokens? Tokens { get; set; }
        
    }

    public class Root
    {
        [JsonPropertyName("data")]
        public Data? Data { get; set; }

      
    }

    public class Tokens
    {
        [JsonPropertyName("TARGET")]
        public string? Target { get; set; }
        
    }
}