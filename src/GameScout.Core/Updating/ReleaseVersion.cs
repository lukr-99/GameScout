using System.Globalization;

namespace GameScout.Core.Updating;

/// <summary>Parses release tags like "v1.2.3" / "1.2" into a comparable <see cref="Version"/>.</summary>
public static class ReleaseVersion
{
    /// <summary>Attempts to parse a tag into a <see cref="Version"/>, ignoring a leading "v".</summary>
    /// <param name="tag">The tag string (e.g. "v0.2.0").</param>
    /// <param name="version">The parsed version on success.</param>
    /// <returns><see langword="true"/> when parsing succeeds.</returns>
    public static bool TryParse(string? tag, out Version version)
    {
        version = new Version(0, 0);
        if (string.IsNullOrWhiteSpace(tag))
            return false;

        string trimmed = tag.Trim().TrimStart('v', 'V');

        // Keep only the numeric "x.y.z" head, dropping any pre-release/build suffix.
        int end = 0;
        while (end < trimmed.Length && (char.IsDigit(trimmed[end]) || trimmed[end] == '.'))
            end++;
        trimmed = trimmed[..end].Trim('.');

        if (trimmed.Length == 0 || !trimmed.Contains('.', StringComparison.Ordinal))
            return int.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out int major)
                ? SetMajor(major, out version)
                : false;

        return Version.TryParse(trimmed, out Version? parsed) && Assign(parsed, out version);
    }

    private static bool SetMajor(int major, out Version version)
    {
        version = new Version(major, 0);
        return true;
    }

    private static bool Assign(Version parsed, out Version version)
    {
        version = parsed;
        return true;
    }
}
