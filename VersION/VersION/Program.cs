using DryIoc;
using LibGit2Sharp;
using McMaster.Extensions.CommandLineUtils;
using VersION.Core;
using VersION.Core.Extensibility;
using static System.Console;

var app = new CommandLineApplication();
app.HelpOption();

var progress = new Progress<ISourceFolder.SourceFolderScanProgress>();
progress.ProgressChanged += ProgressOnProgressChanged;

await Orchestrator.Initialize(Environment.CurrentDirectory, CancellationToken.None, progress);

var folder = Orchestrator.Container.Resolve<ISourceFolder>();

WriteLine($"Name: {folder.Name}");
WriteLine($"Git Usage: {folder.GitUtilization}");
WriteLine($"Projects: {folder.Projects.Count}");

WriteLine();

foreach (var repo in folder.Repositories.Values)
{
    var actual = await repo;
    WriteLine($"Repository: {actual.Info.Path} - {actual.Commits.QueryBy(new CommitFilter(){SortBy = CommitSortStrategies.Time}).Count()} commits on {actual.Branches.Count()} branches.");
}

foreach (var project in folder.Projects)
{
    WriteLine($"Project: {project.Name} @ {(await project.Repository).Info.Path}");
}

WriteLine();
WriteLine("Press any button to exit.");
Orchestrator.Shutdown();
ReadLine();

void ProgressOnProgressChanged(object? sender, ISourceFolder.SourceFolderScanProgress progress)
{
    var pos = (CursorLeft, CursorTop);
    WriteLine($"State: {progress.State}, Projects analyzed: {progress.ProjectsAnalyzed}");
    SetCursorPosition(pos.CursorLeft, pos.CursorTop);
}