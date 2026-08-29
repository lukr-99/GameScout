using System.Net.Http;
using GameScout.Core.Abstractions;

namespace GameScout.Core.Net;

/// <summary>
/// Default <see cref="IHttpTextClient"/> backed by a shared <see cref="HttpClient"/>.
/// The caller owns the <see cref="HttpClient"/> lifetime.
/// </summary>
public sealed class HttpTextClient : IHttpTextClient
{
    private readonly HttpClient _http;

    /// <summary>Initializes a new <see cref="HttpTextClient"/> over <paramref name="http"/>.</summary>
    /// <param name="http">The underlying client. A short timeout and a User-Agent are recommended.</param>
    public HttpTextClient(HttpClient http)
        => _http = http ?? throw new ArgumentNullException(nameof(http));

    /// <inheritdoc/>
    public async Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await _http
            .GetAsync(url, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }
}
