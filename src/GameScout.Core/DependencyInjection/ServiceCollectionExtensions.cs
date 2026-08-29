using GameScout.Core.Abstractions;
using GameScout.Core.Aggregation;
using GameScout.Core.Sources.CheapShark;
using GameScout.Core.Sources.Epic;
using GameScout.Core.Sources.GamerPower;
using GameScout.Core.Updating;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GameScout.Core.DependencyInjection;

/// <summary>
/// Composable registration for the platform-neutral Core services. The host is responsible for
/// registering an <see cref="IHttpTextClient"/> before building the provider.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the giveaway/deal sources and their aggregators. Requires an
    /// <see cref="IHttpTextClient"/> to be registered by the caller.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional callback to adjust locale/country.</param>
    /// <returns>The same collection, for chaining.</returns>
    public static IServiceCollection AddGameScoutCore(
        this IServiceCollection services,
        Action<GameScoutOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new GameScoutOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);
        services.TryAddSingleton(TimeProvider.System);

        // Free-game giveaway sources.
        services.AddSingleton<IGiveawaySource>(sp =>
            new EpicFreeGamesSource(sp.GetRequiredService<IHttpTextClient>(), options.Locale, options.Country));
        services.AddSingleton<IGiveawaySource>(sp =>
            new GamerPowerSource(sp.GetRequiredService<IHttpTextClient>()));

        // On-sale deal sources.
        services.AddSingleton<IDealSource>(sp =>
            new CheapSharkSource(sp.GetRequiredService<IHttpTextClient>()));

        // Aggregators (deps resolved from the registrations above).
        services.AddSingleton(sp => new GiveawayAggregator(
            sp.GetServices<IGiveawaySource>(),
            sp.GetRequiredService<TimeProvider>(),
            options.MinimumWorth));
        services.AddSingleton<DealAggregator>();

        // Update checking (GitHub releases).
        services.AddSingleton<IReleaseSource>(sp =>
            new GitHubReleaseSource(sp.GetRequiredService<IHttpTextClient>()));
        services.AddSingleton<UpdateChecker>();

        return services;
    }
}
