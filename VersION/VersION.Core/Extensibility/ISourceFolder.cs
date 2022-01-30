using LibGit2Sharp;

namespace VersION.Core.Extensibility;

///-------------------------------------------------------------------------------------------------
/// <summary>
///     Interface which provides information and interoperability with a source folder (identical
///     to working directories) for any VersION operations.
/// </summary>
///-------------------------------------------------------------------------------------------------
public interface ISourceFolder : IDisposable
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Gets the full pathname of the source folder.    </summary>
    ///
    /// <value> The full pathname of the source folder. </value>
    ///-------------------------------------------------------------------------------------------------
    string FullPath { get; }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Gets the name of this source folder.  </summary>
    ///
    /// <value> The name.   </value>
    ///-------------------------------------------------------------------------------------------------
    string Name { get; }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Gets the projects of this source folder.  </summary>
    ///
    /// <value> The projects.   </value>
    ///-------------------------------------------------------------------------------------------------
    IReadOnlyList<IProject> Projects { get; }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Gets a value indicating whether this ISourceFolder is using Git as source management
    ///     system.
    /// </summary>
    ///
    /// <value> True if this ISourceFolder is Git powered, false if not.    </value>
    ///-------------------------------------------------------------------------------------------------
    bool IsGitPowered { get; }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Gets the Git utilization.   </summary>
    ///
    /// <value> The Git utilization.    </value>
    ///-------------------------------------------------------------------------------------------------
    GitUtilization GitUtilization { get; }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Gets the primary repository.    </summary>
    ///
    /// <value> The primary repository. </value>
    ///-------------------------------------------------------------------------------------------------
    AsyncLazy<Repository> PrimaryRepository { get; }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Gets the repositories in case the source folder is a multi repository source folder.
    /// </summary>
    ///
    /// <value> The repositories.   </value>
    ///-------------------------------------------------------------------------------------------------
    IReadOnlyList<AsyncLazy<Repository>> Repositories { get; }

    /// <summary>   (Immutable) a source folder scan progress.  </summary>
    public record SourceFolderScanProgress(string State, int ProjectsAnalyzed);
}