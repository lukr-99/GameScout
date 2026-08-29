namespace GameScout.Core.Games;

/// <summary>
/// A normally-paid game currently discounted (not free), normalized across storefronts.
/// </summary>
/// <param name="Title">Display title of the game.</param>
/// <param name="Store">Storefront offering the deal.</param>
/// <param name="SalePrice">Formatted current price (e.g. "$11.99").</param>
/// <param name="NormalPrice">Formatted normal/list price (e.g. "$39.99").</param>
/// <param name="DiscountPercent">Whole-number discount (e.g. 70 for 70% off).</param>
/// <param name="Url">Link to the deal/redirect page, if known.</param>
/// <param name="ImageUrl">A cover/thumbnail image URL, if known.</param>
/// <param name="SaleAmount">Numeric current price, used for sorting; null when unknown.</param>
public sealed record GameDeal(
    string Title,
    GameStore Store,
    string SalePrice,
    string NormalPrice,
    int DiscountPercent,
    string? Url = null,
    string? ImageUrl = null,
    decimal? SaleAmount = null);
