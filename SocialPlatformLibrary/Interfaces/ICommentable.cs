using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Posts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Interfaces;

/// <summary>
/// Marks a post as commentable.
/// Any class implementing this can have comments attached to it.
/// </summary>
public interface ICommentable
{
    /// <summary>Comments attached to this post, in insertion order.</summary>
    List<Comment> Comments { get; }

    /// <summary>Attaches a comment to this post.</summary>
    void AddComment(Comment comment);

    /// <summary>Detaches the comment with the given ID from this post.</summary>
    void RemoveCommentById(Guid commentId);
}
