using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Interfaces;

/// <summary>
/// Marks a post as likable.
/// Any class implementing this can be liked or unliked by users.
/// </summary>
public interface ILikable
{
    /// <summary>Set of user IDs who have liked this post. 
    /// HashSet prevents duplicate likes.</summary>
    HashSet<Guid> Likes { get; }

    /// <summary>Adds a like if the user hasn't liked it yet;
    /// removes it if they already have.</summary>
    void ToggleLike(Guid userId);
}
