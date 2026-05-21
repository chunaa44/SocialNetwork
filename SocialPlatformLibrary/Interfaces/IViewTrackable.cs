using System;

namespace SocialPlatformLibrary.Interfaces;

/// <summary>
/// Marks a post as view-trackable. 
/// Any class implementing this records unique viewers.
/// </summary>
public interface IViewTrackable
{
    /// <summary>Set of user IDs who have viewed this post.
    /// HashSet prevents counting the same user twice.</summary>
    HashSet<Guid> Viewers { get; }

    /// <summary>
    /// Records a view for the given user.
    /// Returns true if this was a new unique view; false if the user already viewed it.
    /// </summary>
    bool AddView(Guid userId);
}
