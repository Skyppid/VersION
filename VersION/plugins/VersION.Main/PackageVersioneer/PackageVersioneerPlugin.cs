using DryIoc;
using Spectre.Console.Cli;
using VersION.Core.Extensibility;
using VersION.Main.PackageVersioneer.Commands;

namespace VersION.Main.PackageVersioneer
{
    public sealed class PackageVersioneerPlugin : IPlugin
    {
        public string Name => "Package Versioneer";
        
        public async Task Initialize(IContainer container)
        {
            container.Register(typeof(AsyncCommand), typeof(VersioneerCommand));
        }
    }
}
