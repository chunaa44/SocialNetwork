using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Interfaces;

/// <summary>
/// Marks a post as bookmarkable. Any class implementing this can be saved by users for later.
/// </summary>
public interface IBookmarkable
{
    /// <summary>Set of user IDs who have bookmarked this post. 
    /// HashSet prevents duplicates.</summary>
    HashSet<Guid> Bookmarks { get; }

    /// <summary>Adds a bookmark if the user hasn't bookmarked it yet; 
    /// removes it if they already have.</summary>
    void ToggleBookmark(Guid userId);
}
