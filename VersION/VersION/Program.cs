using DryIoc;
using McMaster.Extensions.CommandLineUtils;
using VersION.Core;
using VersION.Core.Extensibility;
using VersION.Core.Management;
using static System.Console;

var app = new CommandLineApplication();
app.HelpOption();

IContainer container = Orchestrator.InitializeContainer();
var sfProvider = container.Resolve<ISourceFolderProvider>();
var progress = new Progress<ISourceFolder.SourceFolderScanProgress>();
progress.ProgressChanged += ProgressOnProgressChanged;

var folder = await sfProvider.Provide(Environment.CurrentDirectory, CancellationToken.None, progress);

WriteLine($"Name: {folder.Name}");
WriteLine($"Git Usage: {folder.GitUtilization}");
WriteLine($"Projects: {folder.Projects.Count}");

WriteLine();

foreach (var project in folder.Projects) 
    WriteLine($"Project: {project.Name} @ {project.PackageVersion}");

WriteLine();
WriteLine("Press any button to exit.");
ReadLine();

void ProgressOnProgressChanged(object sender, ISourceFolder.SourceFolderScanProgress progress)
{
    var pos = (CursorLeft, CursorTop);
    WriteLine($"State: {progress.State}, Projects analyzed: {progress.ProjectsAnalyzed}");
    SetCursorPosition(pos.CursorLeft, pos.CursorTop);
}