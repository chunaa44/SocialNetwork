using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Posts;

/// <summary>
/// A photo post. Supports likes, comments, and bookmarks.
/// </summary>
public class Photo: Post, ILikable, ICommentable, IBookmarkable
{
    public required string PhotoUrl { get; set; }

    // HashSet prevents duplicate likes from the same user
    public HashSet<Guid> Likes { get; } = new HashSet<Guid>();

    public List<Comment> Comments { get; } = new List<Comment>();

    // HashSet prevents a user from bookmarking the same photo twice
    public HashSet<Guid> Bookmarks { get; } = new HashSet<Guid>();

    /// <summary>Adds a like if not already liked; removes it if already liked.</summary>
    public void ToggleLike(Guid userId)
    {
        if (!Likes.Contains(userId)) Likes.Add(userId);
        else Likes.Remove(userId);
    }

    public void AddComment(Comment comment)
    { 
        Comments.Add(comment);
    }

    public void RemoveCommentById(Guid commentId)
    {
        // Find the comment by ID and remove it if it exists
        var comment = Comments.FirstOrDefault(c => c.Id == commentId);
        if(comment != null)
        {
            Comments.Remove(comment);
        }
    }

    /// <summary>Adds a bookmark if not bookmarked; removes it if already bookmarked.</summary>
    public void ToggleBookmark(Guid userId)
    {
        if (!Bookmarks.Contains(userId)) Bookmarks.Add(userId);
        else Bookmarks.Remove(userId);
    }
}
