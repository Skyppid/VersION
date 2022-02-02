using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using DryIoc;
using Spectre.Console;
using Spectre.Console.Cli;
using VersION;
using VersION.Core.Extensibility;

[assembly: InternalsVisibleTo("Tests.VersION")]

var container = new Container();
container.UseInstance(typeof(IContainer), container);
container.Register(typeof(AsyncCommand), typeof(HelpCommand));

// Look up all plugins and add them to the container
string startupLocation = Assembly.GetEntryAssembly()?.Location ??
                         throw new InvalidOperationException("Could not determine startup location.");
string startupDir = Path.GetDirectoryName(startupLocation) ??
                    throw new DirectoryNotFoundException("Could not determine startup directory.");
await foreach (var plugin in PluginOrchestrator.SearchPlugins(startupDir))
    container.Register(typeof(IPlugin), plugin);

foreach (var plugin in container.ResolveMany<IPlugin>())
    await plugin.Initialize(container);

var app = new CommandApp(new TypeRegistrar(container));
app.Configure(c =>
{
    c.AddDelegate("help", c =>
    {
        AnsiConsole.Write("Worked!");
        return 0;
    });
    c.Settings.ExceptionHandler =
        exception =>
        {
            AnsiConsole.WriteException(exception, ExceptionFormats.ShortenEverything);
            return -1;
        };
});

var result = await app.RunAsync(args);
if (args.Length == 0 && Debugger.IsAttached)
{
    var arguments = AnsiConsole.Prompt(new TextPrompt<string>("Please enter your arguments to invoke on VersION:"));
    var startInfo = new ProcessStartInfo("dotnet", $"\"{startupLocation}\" {arguments}")
        {WorkingDirectory = Environment.CurrentDirectory};
    Process.Start(startInfo);
}

return result;

//await Status().StartAsync("Initializing Source Folder", async ctx =>
//{
//    var progress = new Progress<ISourceFolder.SourceFolderScanProgress>();
//    progress.ProgressChanged += (sender, scanProgress) => ctx.Status($"{scanProgress.State} ...").Refresh();

//    await Orchestrator.Initialize(Environment.CurrentDirectory, CancellationToken.None, progress);
//});

//var folder = Orchestrator.Container.Resolve<ISourceFolder>();

//MarkupLine($"Name: [yellow]{folder.Name}[/]");
//MarkupLine($"Git Usage: [red]{folder.GitUtilization}[/]");
//MarkupLine($"Projects: [red]{folder.Projects.Count}[/]");

//WriteLine();

//foreach (var repo in folder.Repositories.Values)
//{
//    var actual = await repo;
//    MarkupLine($"Repository: [orange3]{actual.Info.Path}[/] - [red]{actual.Commits.QueryBy(new CommitFilter(){SortBy = CommitSortStrategies.Time}).Count()}[/] commits on [red]{actual.Branches.Count()}[/] branches.");
//    foreach (var branch in actual.Branches)
//        MarkupLine($"Branch [yellow]{branch.FriendlyName}[/]{(branch.TrackedBranch != null ? " -> [orange3]" + branch.TrackedBranch.FriendlyName + "[/]" : "")} with [red]{branch.Commits.Count()}[/] commits, HEAD: {branch.IsCurrentRepositoryHead}, Behind by: [red]{branch.TrackingDetails.BehindBy}[/], Ahead by: [green1]{branch.TrackingDetails.AheadBy}[/]");

//    WriteLine();
//}

//foreach (var project in folder.Projects)
//{
//    MarkupLine($"Project: [orange3]{project.Name}[/] [red]v{project.PackageVersion}[/] @ [grey74]{(await project.Repository).Info.Path}[/]");
//}

//WriteLine();
//WriteLine("Press any button to exit.");
//Orchestrator.Shutdown();
//System.Console.ReadLine();