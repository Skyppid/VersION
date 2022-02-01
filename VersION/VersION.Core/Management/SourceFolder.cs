using System.Collections.ObjectModel;
using LibGit2Sharp;
using VersION.Core.Extensibility;

namespace VersION.Core.Management;

internal sealed class SourceFolder : ISourceFolder
{
    private SourceFolder(string fullPath, string name)
    {
        FullPath = fullPath;
        Name = name;
    }
    
    private readonly List<IProject> _projects = new();
    private readonly Dictionary<string, AsyncLazy<Repository>> _repositories = new();
    private IReadOnlyDictionary<string, AsyncLazy<Repository>>? _readOnlyRepositories;

    /// <inheritdoc />
    public string FullPath { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public IReadOnlyList<IProject> Projects => _projects.AsReadOnly();

    /// <inheritdoc />
    public bool IsGitPowered => GitUtilization != GitUtilization.None;

    /// <inheritdoc />
    public GitUtilization GitUtilization { get; private set; }

    public AsyncLazy<Repository>? PrimaryRepository { get; private set; }

    public IReadOnlyDictionary<string, AsyncLazy<Repository>> Repositories => _readOnlyRepositories ??= new ReadOnlyDictionary<string, AsyncLazy<Repository>>(_repositories);

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Uses the specified path as the source folder.   </summary>
    ///
    /// <exception cref="DirectoryNotFoundException">   Thrown when <paramref name="path"/> is not a
    ///                                                 valid and existing path.    </exception>
    ///
    /// <param name="path">                 Full pathname of the source folder. </param>
    /// <param name="cancellationToken">    (Optional) A token that allows processing to be
    ///                                     cancelled.  </param>
    /// <param name="progress">             (Optional) The progress.    </param>
    ///
    /// <returns>   An ISourceFolder.   </returns>
    ///-------------------------------------------------------------------------------------------------
    internal static async Task<ISourceFolder> Use(string path, CancellationToken cancellationToken = default, IProgress<ISourceFolder.SourceFolderScanProgress>? progress = null)
    {
        DirectoryInfo di = new(path);
        if (!di.Exists)
            throw new DirectoryNotFoundException($"The provided path '{path}' is not valid directory.");

        SourceFolder src = new(di.FullName, di.Name);

        await src.AnalyzeFolder(di, cancellationToken, progress);

        return src;
    }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Analyzes the folder by looking up any existing projects and other metadata.
    /// </summary>
    ///
    /// <param name="directory">            The directory to analyze.   </param>
    /// <param name="cancellationToken">    (Optional) A token that allows processing to be
    ///                                     cancelled.  </param>
    /// <param name="progress">             (Optional) The progress.    </param>
    ///
    /// <returns>   A Task. </returns>
    ///-------------------------------------------------------------------------------------------------
    private async Task AnalyzeFolder(DirectoryInfo directory, CancellationToken cancellationToken = default, IProgress<ISourceFolder.SourceFolderScanProgress>? progress = null)
    {
        progress?.Report(new("Git utilization checkup", 0));
        GitUtilization = LookupGitRoot(directory);
        await ScanProjects(directory, cancellationToken, progress);
    }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Scans all found projects for their metadata.    </summary>
    ///
    /// <param name="directory">            The source folder directory.    </param>
    /// <param name="cancellationToken">    (Optional) A token that allows processing to be
    ///                                     cancelled.  </param>
    /// <param name="progress">             (Optional) The progress.    </param>
    ///
    /// <returns>   A task which completes once all projects were scanned.  </returns>
    ///-------------------------------------------------------------------------------------------------
    private async Task ScanProjects(DirectoryInfo directory, CancellationToken cancellationToken = default, IProgress<ISourceFolder.SourceFolderScanProgress>? progress = null)
    {
        var files = directory.GetFiles("*.csproj", SearchOption.AllDirectories);

        int scanCount = 0;
        List<Task<Project>> projects = new();
        foreach (var file in files)
        {
            if (cancellationToken.IsCancellationRequested) break;
            var task = Project.ScanProject(file, this, cancellationToken);
            projects.Add(task);
            _ = task.ContinueWith(t =>
            {
                progress?.Report(new("Scanning projects",
                    Interlocked.Increment(ref scanCount)));
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        await Task.WhenAll(projects);
        progress?.Report(new("Finished", projects.Count(c => c.IsCompletedSuccessfully)));

        _projects.AddRange(projects.Select(x => x.Result));
    }

    /// -------------------------------------------------------------------------------------------------
    ///  <summary>
    ///      Looks up the parent directories in order to find out whether this source folder is using
    ///      Git.
    ///  </summary>
    /// <param name="directory"></param>
    /// <returns>   True if it uses Git, false if it does not. </returns>
    /// -------------------------------------------------------------------------------------------------
    private GitUtilization LookupGitRoot(DirectoryInfo directory)
    {
        const string gitDir = ".git";

        var gitUtil = GitUtilization.None;
        DirectoryInfo[] matches;

        // Trivial case: .git folder is directly inside source folder
        if ((matches = directory.GetDirectories(gitDir, SearchOption.TopDirectoryOnly)).Any())
        {
            gitUtil = GitUtilization.Full;
            PrimaryRepository = new AsyncLazy<Repository>(() => new(directory.FullName));
            _repositories.Add(matches.First().FullName, PrimaryRepository);
            return gitUtil;
        }

        // Look up the hierarchy for full coverage by a parent .git folder
        DirectoryInfo? current = directory;
        while ((current = current.Parent) != null)
        {
            if ((matches = current.GetDirectories(gitDir, SearchOption.TopDirectoryOnly)).Any())
            {
                gitUtil = GitUtilization.Full;
                PrimaryRepository = new AsyncLazy<Repository>(() => new(current.FullName));
                _repositories.Add(matches.First().FullName, PrimaryRepository);
                break;
            }
        }

        // Iterate through sub directories for partial or multi repository usage
        if (gitUtil == GitUtilization.None)
        {
            var subDirectories = directory.GetDirectories(gitDir, SearchOption.AllDirectories);
            gitUtil = subDirectories.Length == 1 ? GitUtilization.Partial :
                subDirectories.Length > 1 ? GitUtilization.MultiRepo : GitUtilization.None;

            foreach (var subDir in subDirectories)
                _repositories.Add(subDir.FullName, new AsyncLazy<Repository>(() => new Repository(subDir.FullName)));
        }

        return gitUtil;
    }

    public void Dispose()
    {
        foreach (var repo in _repositories.Values.Where(repo => repo.IsValueCreated))
            repo.Value.Result.Dispose();
        _repositories.Clear();
        PrimaryRepository = null!;
    }
}