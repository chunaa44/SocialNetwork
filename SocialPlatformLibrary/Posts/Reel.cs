using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Posts;

/// <summary>
/// A short video (reel) post. Supports reactions and comments.
/// </summary>
public class Reel : Post, IReactable, ICommentable
{
    // NOTE: Only accurate immediately after creation. Once fetched from SQLite,
    // this is empty — Reactions/Comments live in the DB.
    // Use Platform methods directly instead.

    // Keyed by user id — a user can hold at most one reaction at a time
    public Dictionary<Guid, ReactionType> Reactions { get; } = new();
    public List<Comment> Comments { get; } = new List<Comment>();

    /// <summary>Sets the given user's reaction. Setting the same reaction again clears it
    /// (toggle off); setting a different reaction replaces it.</summary>
    public void SetReaction(Guid userId, ReactionType reaction)
    {
        if (Reactions.TryGetValue(userId, out var existing) && existing == reaction)
            Reactions.Remove(userId);
        else
            Reactions[userId] = reaction;
    }

    public void AddComment(Comment comment)
    {
        Comments.Add(comment);
    }

    public void RemoveCommentById(Guid commentId)
    {
        // Find the comment by ID and remove it if it exists
        var comment = Comments.FirstOrDefault(c => c.Id == commentId);
        if (comment != null)
        {
            Comments.Remove(comment);
        }
    }
}