using AozoraSharp.AozoraObjects;
using AozoraSharp.Exceptions;
using AozoraSharp.HttpObjects;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace AozoraSharp;

public class AozoraClient : AozoraObject, IDisposable
{
    /// <summary>
    /// domain of the instance to be connected
    /// </summary>
    public string InstanceDomain { get; }

    /// <summary>
    /// Initialize a new client.
    /// </summary>
    /// <param name="instanceDomain">domain of instance that you want to connect to</param>
    public AozoraClient(string instanceDomain = "bsky.social")
    {
        logger.Info($"Hello AozoraSharp!! Create a session to connect to {instanceDomain}.");
        InstanceDomain = $"https://{instanceDomain}";
        AozoraClientManager.Instance.AddClient(this);
    }

    /// <summary>
    /// current logged in user
    /// </summary>
    public AozoraMyUser CurrentUser { get; private set; }
    /// <summary>
    /// access token
    /// </summary>
    public string AccessToken { get; private set; }
    /// <summary>
    /// Refresh token. Used to refresh the session.
    /// </summary>
    public string RefreshToken { get; private set; }

    internal HttpClient HttpClient { get; } = new();

    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    /// <summary>
    /// Create a new session with ID and password.
    /// </summary>
    /// <param name="id">ID for your account. The domain(e.g. -.bsky.social) must be given.</param>
    /// <param name="password">app password for your account</param>
    /// <returns>your account</returns>
    public async Task<AozoraMyUser> CreateSessionAsync(string id, string password, CancellationToken cancellationToken = default)
    {
        logger.Info($"Creating session as {id}");
        var request = new CreateSessionRequest(id, password);
        var response = await PostCustomXrpcAsync<CreateSessionRequest, CreateSessionResponse>("com.atproto.server.createSession", request, cancellationToken);
        var user = new AozoraMyUser(response, this);
        CurrentUser = user;
        RenewToken(response.AccessJwt, response.RefreshJwt);
        return user;
    }
    /// <summary>
    /// Refresh the token.
    /// </summary>
    public async Task RefreshSessionAsync(CancellationToken cancellationToken = default)
    {
        logger.Info("Refreshing session");
        var response = await PostCustomXrpcWithRefreshTokenAsync<RefreshSessionResponse>("com.atproto.server.refreshSession", cancellationToken);
        RenewToken(response.AccessJwt, response.RefreshJwt);
    }
    /// <summary>
    /// Delete the session.
    /// </summary>
    public async Task DeleteSessionAsync(CancellationToken cancellationToken = default)
    {
        logger.Info("Deleting session");
        await PostCustomXrpcAsync("com.atproto.server.deleteSession", cancellationToken);
    }

    private void RenewToken(string accessToken, string refreshToken)
    {
        logger.Info("Token renewed");
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        HttpClient.DefaultRequestHeaders.Remove("Authorization");
        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
        AozoraClientManager.Instance.SessionKeeper.TokenRenewed(this);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            HttpClient.Dispose();
        }
    }

    /// <summary>
    /// Send a custom GET request.
    /// </summary>
    /// <param name="endpoint">Endpoint path.<br/>e.g. com.atproto.repo.createRecord</param>
    /// <returns>response</returns>
    public async Task<HttpResponseMessage> GetCustomXrpcAsync(string endpoint, CancellationToken cancellationToken = default, params UrlParameter[] parameters)
    {
        return await HttpClient.GetAsync(JoinParameters($"{InstanceDomain}/xrpc/{endpoint}", parameters), cancellationToken);
    }
    public string JoinParameters(string url, UrlParameter[] parameters)
    {
        return parameters.Length > 0 ? $"{url}/?{string.Join("&", parameters)}" : url;
    }
    /// <summary>
    /// Send a custom POST request.
    /// </summary>
    /// <param name="endpoint">Endpoint path.<br/>e.g. com.atproto.repo.createRecord</param>
    /// <param name="value">object to send</param>
    /// <returns>response</returns>
    public async Task<TReturn> PostCustomXrpcAsync<TInvoke, TReturn>(string endpoint, TInvoke value, CancellationToken cancellationToken = default)
    {
        // 戻り値あり，jsonあり
        using var response = await HttpClient.PostAsJsonAsync($"{InstanceDomain}/xrpc/{endpoint}", value, jsonOptions, cancellationToken);
        return await ReadHttpObjectFromResponseAsync<TReturn>(response, cancellationToken);
    }
    /// <summary>
    /// Send a custom POST request.
    /// </summary>
    /// <param name="endpoint">Endpoint path.<br/>e.g. com.atproto.repo.createRecord</param>
    /// <param name="value">object to send</param>
    /// <returns>response</returns>
    public async Task PostCustomXrpcAsync<TInvoke>(string endpoint, TInvoke value, CancellationToken cancellationToken = default)
    {
        // 戻り値なし，jsonあり
        using var response = await HttpClient.PostAsJsonAsync($"{InstanceDomain}/xrpc/{endpoint}", value, jsonOptions, cancellationToken);
        await ThrowIfFailureAsync(response, cancellationToken);
    }
    /// <summary>
    /// Send a custom POST request.
    /// </summary>
    /// <param name="endpoint">Endpoint path.<br/>e.g. com.atproto.repo.createRecord</param>
    /// <returns>response</returns>
    public async Task<TReturn> PostCustomXrpcAsync<TReturn>(string endpoint, CancellationToken cancellationToken = default)
    {
        // 戻り値あり，jsonなし
        using var response = await HttpClient.PostAsync($"{InstanceDomain}/xrpc/{endpoint}", null, cancellationToken);
        return await ReadHttpObjectFromResponseAsync<TReturn>(response, cancellationToken);
    }
    /// <summary>
    /// Send a custom POST request.
    /// </summary>
    /// <param name="endpoint">Endpoint path.<br/>e.g. com.atproto.repo.createRecord</param>
    /// <returns>response</returns>
    public async Task PostCustomXrpcAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        // 戻り値なし，jsonなし
        using var response = await HttpClient.PostAsync($"{InstanceDomain}/xrpc/{endpoint}", null, cancellationToken);
        await ThrowIfFailureAsync(response, cancellationToken);
    }
    /// <summary>
    /// Send a custom POST request.
    /// </summary>
    /// <param name="endpoint">Endpoint path.<br/>e.g. com.atproto.repo.createRecord</param>
    /// <returns>response</returns>
    public async Task<TReturn> PostCustomXrpcAsync<TReturn>(string endpoint, string mimeType, IReadOnlyList<byte> value, CancellationToken cancellationToken = default)
    {
        // 戻り値あり，MIME
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{InstanceDomain}/xrpc/{endpoint}");
        request.Headers.Add("Authorization", $"Bearer {AccessToken}");
        var content = new ByteArrayContent(value as byte[]);
        content.Headers.Add("Content-Type", mimeType);
        request.Content = content;
        using var response = await HttpClient.SendAsync(request, cancellationToken);
        return await ReadHttpObjectFromResponseAsync<TReturn>(response, cancellationToken);
    }
    /// <summary>
    /// Send a custom POST request.
    /// </summary>
    /// <param name="endpoint">Endpoint path.<br/>e.g. com.atproto.repo.createRecord</param>
    /// <returns>response</returns>
    public async Task PostCustomXrpcAsync(string endpoint, string mimeType, IReadOnlyList<byte> value, CancellationToken cancellationToken = default)
    {
        // 戻り値なし，MIME
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{InstanceDomain}/xrpc/{endpoint}");
        request.Headers.Add("Authorization", $"Bearer {AccessToken}");
        var content = new ByteArrayContent(value as byte[]);
        content.Headers.Add("Content-Type", mimeType);
        request.Content = content;
        using var response = await HttpClient.SendAsync(request, cancellationToken);
        await ThrowIfFailureAsync(response, cancellationToken);
    }
    /// <summary>
    /// Send a custom POST request with the refresh token.
    /// </summary>
    /// <param name="endpoint">Endpoint path.<br/>e.g. com.atproto.repo.createRecord</param>
    /// <returns>response</returns>
    private async Task<TReturn> PostCustomXrpcWithRefreshTokenAsync<TReturn>(string endpoint, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{InstanceDomain}/xrpc/{endpoint}");
        request.Headers.Add("Authorization", $"Bearer {RefreshToken}");
        using var response = await HttpClient.SendAsync(request, cancellationToken);
        return await ReadHttpObjectFromResponseAsync<TReturn>(response, cancellationToken);
    }
    public async Task<T> ReadHttpObjectFromResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        await ThrowIfFailureAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(jsonOptions, cancellationToken);
    }
    /// <summary>
    /// Read error from a http response.
    /// </summary>
    /// <param name="response">http response</param>
    /// <returns>read error</returns>
    public async Task<ErrorResponse> ReadErrorFromResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        return await response.Content.ReadFromJsonAsync<ErrorResponse>(jsonOptions, cancellationToken);
    }
    private async Task ThrowIfFailureAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await ReadErrorFromResponseAsync(response, cancellationToken);
            throw new ATProtocolException(error);
        }
    }

    public readonly struct UrlParameter(string name, string value)
    {
        public string Name { get; } = name;
        public string Value { get; } = value;
        public override string ToString() => $"{Name}={Value}";
    }
}
