namespace VersION.Core.Extensibility;

public interface IProject
{
    string Name { get; }

    SemanticVersion PackageVersion { get; }

    Version AssemblyVersion { get; }

    string InformationalVersion { get; }
}