using LibGit2Sharp;
using Version = System.Version;

namespace VersION.Core.Extensibility;

public interface IProject
{
    string Name { get; }

    SemanticVersion PackageVersion { get; }

    Version AssemblyVersion { get; }

    string InformationalVersion { get; }

    ISourceFolder SourceFolder { get; }

    AsyncLazy<Repository> Repository { get; }
}