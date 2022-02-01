using System.Xml;
using VersION.Core.Extensibility;
using LibGit2Sharp;
using Version = System.Version;

namespace VersION.Core.Management;

internal sealed class Project : IProject
{
    private Project()
    {
    }

    public string Name { get; private set; } = default!;
    public SemanticVersion PackageVersion { get; private set; }
    public Version AssemblyVersion { get; private set; } = default!;
    public string InformationalVersion { get; private set; } = default!;
    public ISourceFolder SourceFolder { get; init; } = default!;
    public AsyncLazy<Repository> Repository { get; private set; } = default!;

    internal static async Task<Project> ScanProject(FileInfo file, ISourceFolder sourceFolder, CancellationToken cancellationToken)
    {
        Project project = new()
        {
            SourceFolder = sourceFolder
        };
        await project.ScanFrom(file, cancellationToken);

        project.Repository = sourceFolder.Repositories.MinBy(x => Path.GetRelativePath(file.FullName, x.Key).Length)
            .Value;

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


}