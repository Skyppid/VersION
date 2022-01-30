namespace VersION.Core;

/// <summary>   An enumeration representing different Git usage scenarios. </summary>
public enum GitUtilization
{
    /// <summary>   The source folder does not utilize Git at all.  </summary>
    None = 0,
    /// <summary>   The source folder only partially utilizes Git (in any sub folder).   </summary>
    Partial = 1,
    /// <summary>   The source folder utilizes multiple Git repositories.    </summary>
    MultiRepo = 2,
    /// <summary>   The source folder itself or a parent folder utilizes Git and thus covers the full source folder.  </summary>
    Full = 3
}