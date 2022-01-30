using System.Runtime.CompilerServices;
using DryIoc;
using VersION.Core.Extensibility;
using VersION.Core.Management;

[assembly: InternalsVisibleTo("VersION")]
namespace VersION.Core;

///-------------------------------------------------------------------------------------------------
/// <summary>
///     The orchestrator manages the setup of all services and configures them appropriately for
///     the given source folder.
/// </summary>
///-------------------------------------------------------------------------------------------------
public static class Orchestrator
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Gets the service container. </summary>
    ///
    /// <value> The container.  </value>
    ///-------------------------------------------------------------------------------------------------
    public static IContainer Container { get; } = new Container(Rules.Default);

    public static async Task Initialize(string sourceFolder, CancellationToken cancellationToken, IProgress<ISourceFolder.SourceFolderScanProgress>? progress = null)
    {
        RegisterServices();

        Container.Use(await SourceFolder.Use(sourceFolder, cancellationToken, progress));
    }

    public static void Shutdown()
    {
        Container.Dispose();
    }

    private static void RegisterServices()
    {

    }
}