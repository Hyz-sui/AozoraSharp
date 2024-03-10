using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using AozoraSharp.AozoraObjects;
using AozoraSharp.Constants;
using AozoraSharp.Exceptions;
using AozoraSharp.HttpObjects;

namespace AozoraSharp;

public class AozoraClient : AozoraObject, IDisposable
{
    /// <summary>
    /// instance to be connected
    /// </summary>
    public string InstanceUri { get; }
    public string PublicApi { get; }

    /// <summary>
    /// Initialize a new client.
    /// </summary>
    /// <param name="instanceDomain">domain of instance that you want to connect to</param>
    /// <param name="publicApi">domain of public API</param>
    public AozoraClient(string instanceDomain = "bsky.social", string publicApi = "public.api.bsky.app")
    {
        logger.Info($"Hello AozoraSharp!! Create a session to connect to {instanceDomain}.");
        InstanceUri = $"https://{instanceDomain}";
        AozoraClientManager.Instance.AddClient(this);
        PublicApi = $"https://{publicApi}";
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
    /// <summary>
    /// configurable options for the client
    /// </summary>
    public ClientOption Option { get; set; } = new();

    internal HttpClient HttpClient { get; } = new();

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
        var response = await PostCustomXrpcAsync<CreateSessionRequest, CreateSessionResponse>(ATEndpoint.CreateSession, request, cancellationToken);
        var myProfile = await FetchPublicProfileAsync(id, cancellationToken);
        var user = new AozoraMyUser(response, this, myProfile);
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
        var response = await PostCustomXrpcWithRefreshTokenAsync<RefreshSessionResponse>(ATEndpoint.RefreshSession, cancellationToken);
        RenewToken(response.AccessJwt, response.RefreshJwt);
    }
    /// <summary>
    /// Delete the session.
    /// </summary>
    public async Task DeleteSessionAsync(CancellationToken cancellationToken = default)
    {
        logger.Info("Deleting session");
        await PostCustomXrpcAsync(ATEndpoint.DeleteSession, cancellationToken);
    }

    internal async Task<Profile> FetchPublicProfileAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await GetCustomPublicXrpcAsync<Profile>(ATEndpoint.GetProfile, [new UrlParameter("actor", identifier)], cancellationToken);
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
    public async Task<TReturn> GetCustomXrpcAsync<TReturn>(string endpoint, UrlParameter[] parameters = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetAsync(JoinParameters($"{InstanceUri}/xrpc/{endpoint}", parameters), cancellationToken);
        return await ReadHttpObjectFromResponseAsync<TReturn>(response, cancellationToken);
    }
    // TODO: needs to be refactored
    /// <summary>
    /// Send a custom GET request.
    /// </summary>
    /// <param name="endpoint">Endpoint path.<br/>e.g. com.atproto.repo.createRecord</param>
    /// <returns>response</returns>
    public async Task<TReturn> GetCustomPublicXrpcAsync<TReturn>(string endpoint, UrlParameter[] parameters = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetAsync(JoinParameters($"{PublicApi}/xrpc/{endpoint}", parameters), cancellationToken);
        return await ReadHttpObjectFromResponseAsync<TReturn>(response, cancellationToken);
    }
    public string JoinParameters(string url, UrlParameter[] parameters)
    {
        List<UrlParameter> validParameters = [];
        foreach (var parameter in parameters)
        {
            if (parameter.IsValid())
            {
                validParameters.Add(parameter);
            }
        }
        return validParameters.Count > 0 ? $"{url}?{string.Join("&", validParameters)}" : url;
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
        using var response = await HttpClient.PostAsJsonAsync($"{InstanceUri}/xrpc/{endpoint}", value, CommonConstant.DefaultJsonOptions, cancellationToken);
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
        using var response = await HttpClient.PostAsJsonAsync($"{InstanceUri}/xrpc/{endpoint}", value, CommonConstant.DefaultJsonOptions, cancellationToken);
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
        using var response = await HttpClient.PostAsync($"{InstanceUri}/xrpc/{endpoint}", null, cancellationToken);
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
        using var response = await HttpClient.PostAsync($"{InstanceUri}/xrpc/{endpoint}", null, cancellationToken);
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
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{InstanceUri}/xrpc/{endpoint}");
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
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{InstanceUri}/xrpc/{endpoint}");
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
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{InstanceUri}/xrpc/{endpoint}");
        request.Headers.Add("Authorization", $"Bearer {RefreshToken}");
        using var response = await HttpClient.SendAsync(request, cancellationToken);
        return await ReadHttpObjectFromResponseAsync<TReturn>(response, cancellationToken);
    }
    public async Task<T> ReadHttpObjectFromResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        await ThrowIfFailureAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(CommonConstant.DefaultJsonOptions, cancellationToken);
    }
    /// <summary>
    /// Read error from a http response.
    /// </summary>
    /// <param name="response">http response</param>
    /// <returns>read error</returns>
    public async Task<ErrorResponse> ReadErrorFromResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        return await response.Content.ReadFromJsonAsync<ErrorResponse>(CommonConstant.DefaultJsonOptions, cancellationToken);
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
        public override string ToString() => $"{Name.ToLower()}={Value.ToLower()}";
        public bool IsValid() => Name != null && Value != null;
    }

    public sealed class ClientOption
    {
        /// <summary>
        /// Identifier string for your application. <br/>
        /// This is unofficial and not defined by lexicon, but used as application identifier by several third-party applications.
        /// </summary>
        public string PostVia { get; set; } = CommonConstant.DefaultPostVia;
    }
}
