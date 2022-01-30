using System.Xml;
using VersION.Core.Extensibility;

namespace VersION.Core.Management;

internal sealed class Project : IProject
{
    private Project()
    {
    }

    internal static async Task<Project> ScanProject(FileInfo file, CancellationToken cancellationToken)
    {
        Project project = new();
        await project.ScanFrom(file, cancellationToken);
        return project;
    }

    private async Task ScanFrom(FileInfo file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenRead();

        XmlDocument doc = new();
        doc.Load(stream);

        var properties = doc.SelectNodes("Project/PropertyGroup/*")?.OfType<XmlElement>().ToArray();
        if (properties == null) return;

        Name = properties.FirstOrDefault(p => p.Name == "Name")?.InnerText ??
               file.Name.Substring(0, file.Name.LastIndexOf('.'));
        TryMatchVersion(properties.FirstOrDefault(p =>  p.Name == "AssemblyVersion"));
        TryMatchSemanticVersion(properties.FirstOrDefault(p => p.Name == "PackageVersion"));
    }

    private void TryMatchSemanticVersion(XmlElement? packageVersionProperty)
    {
        PackageVersion = SemanticVersion.TryParse(packageVersionProperty?.InnerText, out var version)
            ? version
            : new(1, 0, 0);
    }

    private void TryMatchVersion(XmlElement? versionProperty)
    {
        AssemblyVersion = Version.TryParse(versionProperty?.InnerText, out var version) ? version : new Version(1, 0);
    }

    public string Name { get; private set; } = default!;
    public SemanticVersion PackageVersion { get; private set; }
    public Version AssemblyVersion { get; private set; }
    public string InformationalVersion { get; private set; }
}