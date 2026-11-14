namespace GameScout.Core.About;

/// <summary>One line in the credits and licences list.</summary>
/// <param name="Name">What is credited, such as a library or data provider.</param>
/// <param name="Role">What GameScout uses it for.</param>
/// <param name="License">Licence or terms note.</param>
public sealed record CreditEntry(string Name, string Role, string License);
