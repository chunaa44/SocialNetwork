using SocialPlatformLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SocialPlatformLibrary.Posts;

/// <summary>
/// A comment attached to a parent post (Photo, Reel, or another Comment).
/// Supports likes.
/// </summary>
public class Comment: Post, ILikable
{
    // ID of the post  this comment belongs to
    public required Guid ParentId{ get; init; }

    // HashSet prevents duplicate likes from the same user
    public HashSet<Guid> Likes { get; } = new HashSet<Guid>();

    /// <summary>Adds a like if not already liked; removes it if already liked.</summary>
    public void ToggleLike(Guid userId)
    {
        if (!Likes.Contains(userId)) Likes.Add(userId);
        else Likes.Remove(userId);
    }

}
