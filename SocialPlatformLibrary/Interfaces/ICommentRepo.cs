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

    /// <summary>Sets a user's reaction. Setting the same reaction the user already has
    /// removes it (toggle off); setting a different reaction replaces it.</summary>
    void SetReaction(Guid id, Guid userId, ReactionType reaction);

    /// <summary>Returns all reactions on this comment, keyed by user id.</summary>
    Dictionary<Guid, ReactionType> GetReactions(Guid id);
}