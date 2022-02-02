using System.Text.RegularExpressions;

namespace VersION.Core
{
    /// <summary>   A semantic version. </summary>
    public record struct SemanticVersion(int Major, int Minor, int Patch, string? PreRelease = null, string? Build = null)
    {
        private const string Expr =
            @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";

        public static SemanticVersion Default => new(1, 0, 0);

        public static SemanticVersion Parse(string versionString)
        {
            var match = Regex.Match(versionString, Expr);
            if (!match.Success)
                throw new ArgumentException("The provided string does not represent a valid semantic version.", nameof(versionString));

            int major = int.Parse(match.Groups[1].Value);
            int minor = int.Parse(match.Groups[2].Value);
            int patch = int.Parse(match.Groups[3].Value);

            string? preRelease = match.Groups[4].Value;
            if (string.IsNullOrEmpty(preRelease))
                preRelease = null;

            string? hash = match.Groups[5].Value;
            if (string.IsNullOrEmpty(hash))
                hash = null;

            return new SemanticVersion(major, minor, patch, preRelease, hash);
        }

        public static bool TryParse(string? versionString, out SemanticVersion version)
        {
            version = Default;
            if (string.IsNullOrEmpty(versionString))
                return false;

            var match = Regex.Match(versionString, Expr);
            if (!match.Success)
                return false;

            if (!int.TryParse(match.Groups[1].Value, out int major))
                return false;
            if (!int.TryParse(match.Groups[2].Value, out int minor))
                return false;
            if (!int.TryParse(match.Groups[3].Value, out int patch))
                return false;
            string? preRelease = match.Groups[4].Value;
            if (string.IsNullOrEmpty(preRelease))
                preRelease = null;

            string? hash = match.Groups[5].Value;
            if (string.IsNullOrEmpty(hash))
                hash = null;

            version = new SemanticVersion(major, minor, patch, preRelease, hash);
            return true;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}{(string.IsNullOrEmpty(PreRelease) ? "" : "-" + PreRelease)}{(string.IsNullOrEmpty(Build) ? "" : "+" + Build)}";
        }
    }
}
