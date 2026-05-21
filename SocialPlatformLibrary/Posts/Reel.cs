using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Posts;

/// <summary>
/// A short video (reel) post. Supports likes and comments.
/// </summary>
public class Reel: Post, ILikable, ICommentable
{
    // HashSet prevents duplicate likes from the same user
    public HashSet<Guid> Likes { get; } = new HashSet<Guid>();
    public List<Comment> Comments { get; } = new List<Comment>();

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
}
