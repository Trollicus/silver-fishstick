using System.Text.Json.Serialization;

namespace LinkvertiseBypass.Handlers.Json;

public class Response
{
    public class Data
    {
        [JsonPropertyName("link")]
        public Link? Link { get; set; }
    }

    public class Link
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("target_type")]
        public string? TargetType { get; set; }


        [JsonPropertyName("target_host")]
        public string? TargetHost { get; set; }

    }





    public class Root
    {
        
        [JsonPropertyName("data")]
        public Data data { get; set; }


        public string user_token { get; set; }
    }



}