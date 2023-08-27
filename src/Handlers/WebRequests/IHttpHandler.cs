using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;

namespace LinkvertiseBypass.Handlers.WebRequests;

/// <summary>
/// A class that handles HTTP requests with optional proxy support.
/// </summary>
public class HttpHandler
{
    private readonly HttpClient? _client;

    /// <summary>
    /// Initializes a new instance of the HttpHandler class with optional proxy configuration.
    /// </summary>
    /// <param name="proxyHost">The proxy host, if a proxy should be used.</param>
    /// <param name="proxyPort">The proxy port, if a proxy should be used.</param>
    public HttpHandler(string proxyHost = "", int proxyPort = 0)
    {
        var clientHandler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.Brotli | DecompressionMethods.GZip | DecompressionMethods.Deflate,
           SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
           ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
        };

        if (!string.IsNullOrEmpty(proxyHost) && proxyPort > 0)
        {
            clientHandler.Proxy = new WebProxy(proxyHost, proxyPort);
            clientHandler.UseProxy = true;
        }

        _client = new HttpClient(clientHandler, true);
    }
    
    /// <summary>
    /// Asynchronously sends a POST request to the specified URI.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="method">The HTTP method to use for the request.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the HttpResponseMessage.</returns>
    public async Task<HttpResponseMessage> PostAsync(string uri, HttpMethod method)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(uri),
            Method = method,
        };

        Debug.Assert(_client != null, nameof(_client) + " != null");
        return await _client.SendAsync(request);
    }

    /// <summary>
    /// Asynchronously sends a POST request with JSON content to the specified URI.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="method">The HTTP method to use for the request.</param>
    /// <param name="json">The JSON content to include in the request body.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the HttpResponseMessage.</returns>
    public async Task<HttpResponseMessage> PostAsync(string uri, HttpMethod method,
        string json)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(uri),
            Method = method,
            Content = new StringContent(json)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue("application/json")
                }
            },
        };

        Debug.Assert(_client != null, nameof(_client) + " != null");
        return await _client.SendAsync(request);
    }

    /// <summary>
    /// Asynchronously sends a POST request with JSON content and custom headers to the specified URI.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="method">The HTTP method to use for the request.</param>
    /// <param name="json">The JSON content to include in the request body.</param>
    /// <param name="requestHeaders">An array of RequestHeadersEx objects representing custom headers for the request.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the HttpResponseMessage.</returns>
    public async Task<HttpResponseMessage> PostAsync(string uri, HttpMethod method,
        string json, RequestHeadersEx[] requestHeaders)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(uri),
            Method = method,
            Content = new StringContent(json)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue("application/json")
                }
            },
        };
        

        foreach (var requestHeader in requestHeaders)
        {
            request.Headers.Add(requestHeader.Key, requestHeader.Value);
        }


        Debug.Assert(_client != null, nameof(_client) + " != null");
        return await _client.SendAsync(request);
    }
    
    /// <summary>
    /// Asynchronously sends a POST request with custom headers to the specified URI.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="method">The HTTP method to use for the request.</param>
    /// <param name="requestHeaders">An array of RequestHeadersEx objects representing custom headers for the request.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the HttpResponseMessage.</returns>
    public async Task<HttpResponseMessage> PostAsync(string uri, HttpMethod method,
         RequestHeadersEx[] requestHeaders)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(uri),
            Method = method
        };
        
        foreach (var requestHeader in requestHeaders)
        {
            request.Headers.Add(requestHeader.Key, requestHeader.Value);
        }

        Debug.Assert(_client != null, nameof(_client) + " != null");
        return await _client.SendAsync(request);
    }

    /// <summary>
    /// Asynchronously downloads a file from the specified URL and saves it to the specified path.
    /// </summary>
    /// <param name="url">The URL of the file to download.</param>
    /// <param name="path">The local file path where the file should be saved.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DownloadFileAsync(string url, string path)
    {
        Debug.Assert(_client != null, nameof(_client) + " != null");
        var streamAsync = await _client.GetStreamAsync(url);
    
        // Use 'using' statement to properly dispose the file stream after the method execution
        await using var fileStream = File.Create(path);
        await streamAsync.CopyToAsync(fileStream);
    }


    /// <summary>
    /// Asynchronously gets the content of the specified URL as a string.
    /// </summary>
    /// <param name="url">The URL to get the content from.</param>
    /// <returns>A task representing the asynchronous operation, with a result containing the content as a string.</returns>
    public async Task<string> GetStringAsync(string url)
    {
        Debug.Assert(_client != null, nameof(_client) + " != null");
        return await _client.GetStringAsync(url);
    }
    
    public record RequestHeadersEx(string Key, string? Value);
}
