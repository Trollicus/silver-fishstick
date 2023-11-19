using System.Text.Json.Serialization;

namespace LinkvertiseBypass.Handlers.Json {
    public class FinalResponse : BaseRequest {}

    public class AdAccessToken : BaseRequest {}

    public class AdCompletedToken : BaseRequest
    {
        public class CompleteDetailPageContent
        {
            [JsonPropertyName("access_token")]
            public required string? access_token { get; set; }
        }
    }
}