using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using LinkvertiseBypass.Handlers.Json;
using LinkvertiseBypass.Handlers.WebRequests;

namespace LinkvertiseBypass.Linkvertise;

public class Linkvertise
{
	private readonly HttpHandler _httpHandler = new();

	public async Task Bypass(Uri uri)
	{
		var (id, name) = uri.AbsolutePath.Trim('/').Split('/') switch
		{
			{ Length: >= 2 } segments => (segments[0], segments[1]),
			_ => throw new Exception("Invalid path segments")
		};

		Console.WriteLine("Attempting Phase 1...");

		var phase1 = await _httpHandler.PostAsync(
			$"https://publisher.linkvertise.com/api/v1/redirect/link/static/{id}/{name}", HttpMethod.Get,
			new[]
			{
				new HttpHandler.RequestHeadersEx("User-Agent",
					"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/116.0"),
				new HttpHandler.RequestHeadersEx("Connection", "close"),
				new HttpHandler.RequestHeadersEx("Accept", "application/json")
			});

		var response1 = await phase1.Content.ReadAsStringAsync();

		var userToken = "";
		var dlid = 0;
		var targetType = "";

		if (JsonHelper.TryDeserialize<Response.Root>(response1, out var data) && data != null)
		{
			userToken = data.user_token;
			Debug.Assert(data.data.Link != null, "data.data.Link != null");
			dlid = data.data.Link.Id;
			targetType = data.data.Link.TargetType == "PASTE" || data.data.Link.TargetHost == "linkvertise.com" ? "paste" : "target";


		}

		Console.WriteLine("Attempting Phase 2...");

		var phase2 = await _httpHandler.PostAsync(
			$"https://obseu.bizseasky.com/ct?id=14473&url={HttpUtility.UrlEncode(uri.ToString())}", HttpMethod.Get,
			new[]
			{
				new HttpHandler.RequestHeadersEx("User-Agent",
					"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/116.0"),
				new HttpHandler.RequestHeadersEx("Accept", "application/json")
			});

		var response2 = await phase2.Content.ReadAsStringAsync();

		Match match = Regex.Match(response2, "\"jsonp\":\"([^\"]+)\"");

		var jsonp = match.Groups[1].Value;


		Console.WriteLine("Attempting Phase 3...");


		var phase3 = await _httpHandler.PostAsync(
			$"https://publisher.linkvertise.com/api/v1/redirect/link/{id}/{name}/traffic-validationv2?X-Linkvertise-UT={userToken}",
			HttpMethod.Post, $"{{\"token\":\"{jsonp}\",\"type\":\"cq\"}}");

		var response3 = await phase3.Content.ReadAsStringAsync();

		var targets = "";

		if (JsonHelper.TryDeserialize<Phase3.Root>(response3, out var hPhase3) && data != null)
		{
			targets = hPhase3?.Data?.Tokens?.Target;
		}

		Console.WriteLine("Attempting Phase 4...");

		var phase4 = await _httpHandler.PostAsync(
			$"https://publisher.linkvertise.com/api/v1/redirect/link/{id}/{name}/{targetType}?X-Linkvertise-UT={userToken}",
			HttpMethod.Post,
			$"{{\"serial\": \"{Convert.ToBase64String(Encoding.UTF8.GetBytes($"{{\"link_id\": {dlid}, \"timestamp\": \"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}\", \"random\":6548307}}"))}\", \"token\": \"{targets}\"}}",
			new[]
			{
				new HttpHandler.RequestHeadersEx("User-Agent",
					"curl/7.54.1"),
				new HttpHandler.RequestHeadersEx("Accept", "application/json")
			});

		var response4 = await phase4.Content.ReadAsStringAsync();

		if (JsonHelper.TryDeserialize<Phase4.Root>(response4, out var uPhase4) && uPhase4 != null)
		{
			Console.WriteLine(uPhase4.Data?.Target ?? uPhase4.Data?.Paste);

		}

		Console.ReadLine();
	}
}