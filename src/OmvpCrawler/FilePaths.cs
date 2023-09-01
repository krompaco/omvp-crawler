using System.Text.RegularExpressions;

namespace OmvpCrawler;

public static class FilePaths
{
    public static string GetSafeFileName(string name)
    {
        var invalids = Path.GetInvalidFileNameChars();
        var safe = new string(name.Select(c => invalids.Contains(c) ? '-' : c).ToArray()).Trim('-');

        return Regex
            .Replace(safe, "--+", "-")
            .Trim('-');
    }
}