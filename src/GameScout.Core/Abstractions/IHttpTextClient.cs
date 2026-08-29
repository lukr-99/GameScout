namespace GameScout.Core.Abstractions;

/// <summary>
/// Minimal HTTP transport used by giveaway sources. Abstracted so sources can be unit-tested
/// against canned payloads without touching the network (see the testing rules in RULES.md).
/// </summary>
public interface IHttpTextClient
{
    /// <summary>Performs a GET and returns the response body as a string.</summary>
    /// <param name="url">Absolute request URL.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>The response body.</returns>
    Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default);
}
