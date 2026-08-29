namespace GameScout.Core.Tests.TestSupport;

/// <summary>Loads the JSON fixtures copied next to the test assembly.</summary>
internal static class Samples
{
    public static string Load(string fileName)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Samples", fileName);
        return File.ReadAllText(path);
    }

    public static string EpicPromotions() => Load("epic-promotions.json");

    public static string GamerPowerSteam() => Load("gamerpower-steam.json");
}
