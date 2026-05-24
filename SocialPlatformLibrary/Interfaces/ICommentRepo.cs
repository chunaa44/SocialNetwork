using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Posts;
using System;
using System.Collections.Generic;

namespace SocialPlatformLibrary.Interfaces;

public interface ICommentRepo
{
    /// <summary>Creates a new comment and returns the created instance.</summary>
    public Comment CreateComment(CommentDTO comment);

    /// <summary>Removes the comment with the given ID. Returns true if removed.</summary>
    public bool RemoveCommentById(Guid id);

    /// <summary>Updates content of an existing comment. Returns null if not found.</summary>
    public Comment UpdateCommentById(Guid id, string newContent);

    /// <summary>Returns the comment with the given ID, or null if not found.</summary>
    public Comment GetCommentById(Guid id);

    /// <summary>Returns all comments in the store.</summary>
    public List<Comment> GetAllComments();

    void ToggleLike(Guid id, Guid userId);

    HashSet<Guid> GetLikes(Guid id);
}
