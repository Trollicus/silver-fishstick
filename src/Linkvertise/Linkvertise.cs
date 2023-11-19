using System.Text.Json;
using LinkvertiseBypass.Handlers.Json;
using LinkvertiseBypass.Handlers.WebRequests;

namespace LinkvertiseBypass.Linkvertise;

public class Linkvertise
{
	private readonly HttpHandler _httpHandler = new();

    private readonly HttpHandler.RequestHeadersEx[] _headers =
    {
        new("Origin", "https://linkvertise.com"),
        new("Referer", "https://linkvertise.com/"),
        new("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv,109.0) Gecko/20100101 Firefox/119.0"),
        new("Accept", "application/json"),
        new("Accept-Language", "en-CA,en-US;q=0.7,en;q=0.3"),
        new("Sec-Fetch-Dest", "empty"),
        new("Sec-Fetch-Mode", "cors"),
        new("Sec-Fetch-Site", "same-site")
    };

    private (string id, string name) ParseUrl(Uri uri)
    {
        string id, name;
        string[] segments = uri.AbsolutePath.Trim('/').Split('/');

        if (segments.Length < 2)
        {
            throw new Exception("Invalid path segments");
        }

        if (uri.AbsolutePath.Contains("download"))
        {
            id = segments[1];
            name = segments[2];
        }
        else
        {
            id = segments[0];
            name = segments[1];
        }

        return (id, name);
    }

    public async Task<string> ExecuteGraphQlRequestAsync<T>(T requestData, string endpoint, Func<JsonElement, string> resultSelector)
    {
        JsonDocument jsonData = JsonSerializer.SerializeToDocument(requestData);
        string requestJson = jsonData.RootElement.GetProperty("root").ToString();

        HttpResponseMessage responseData = await _httpHandler.PostAsync(endpoint, HttpMethod.Post, requestJson, _headers);
        string response = responseData.Content.ReadAsStringAsync().Result;

        return resultSelector(JsonDocument.Parse(response).RootElement.GetProperty("data"));
    }

    public T CreateRequestData<T>(string userId, string url, string? accessToken = null, string? adCompletedToken = null) where T : BaseRequest
    {
        T request = Activator.CreateInstance<T>();
        request.root = new BaseRequest.RootMain
        {
            OperationName = BaseRequest.GetOperation<T>(),
            Query = BaseRequest.GetMutationQuery<T>(),
            Variables = new BaseRequest.Variables
            {
                LinkIdentification = new BaseRequest.LinkIdentification
                {
                    UserIdAndUrl = new BaseRequest.UserIdAndUrl
                    {
                        UserId = userId,
                        Url = url
                    }
                },
                Origin = "https://linkvertise.com/",
                AdditionalData = new BaseRequest.AdditionalData
                {
                    Taboola = new BaseRequest.Taboola
                    {
                        UserId = "",
                        Url = ""
                    }
                }
            }
        };

        if (typeof(T) == typeof(AdCompletedToken))
        {
            request.root.Variables.CompleteDetailPageContent = new AdCompletedToken.CompleteDetailPageContent
            {
                access_token = accessToken
            };
        }
        else if (typeof(T) == typeof(FinalResponse))
        {
            request.root.Variables.Token = adCompletedToken;
        }

        return request;
    }


    public async Task Bypass(Uri uri)
	{
        
        (string id, string name) = ParseUrl(uri);

        // Part 1: Grab Ads Access Tokens
        Console.WriteLine("Attempting Phase 1...");

        AdAccessToken accessData = CreateRequestData<AdAccessToken>(id, name);
        string adToken = await ExecuteGraphQlRequestAsync(accessData, "https://publisher.linkvertise.com/graphql", data => data.GetProperty("getDetailPageContent").GetProperty("access_token").ToString());

        // Part 2 Get Ad Completed Token
        Console.WriteLine("Attempting Phase 2...");

        AdCompletedToken adCompletedData = CreateRequestData<AdCompletedToken>(id, name, adToken);
        string adCompletedToken = await ExecuteGraphQlRequestAsync(adCompletedData, "https://publisher.linkvertise.com/graphql", data => data.GetProperty("completeDetailPageContent").GetProperty("TARGET").ToString());
        
        // Part 3 Get Final Url
        Console.WriteLine("Attempting Phase 3...");

        FinalResponse finalResponse = CreateRequestData<FinalResponse>(id, name, null, adCompletedToken);
        string finalUrl = await ExecuteGraphQlRequestAsync(finalResponse, "https://publisher.linkvertise.com/graphql", data => data.GetProperty("getDetailPageTarget").GetProperty("url").ToString());

        Console.WriteLine(finalUrl);

        Console.ReadLine();
	}
}