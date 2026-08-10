using SocialPlatformLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SocialPlatformLibrary.Posts;

/// <summary>
/// A comment attached to a parent post.
/// Supports reactions.
/// </summary>
public class Comment : Post, IReactable
{
    // ID of the post  this comment belongs to
    public required Guid ParentId { get; init; }

    // NOTE: Only accurate immediately after creation. Once fetched from SQLite,
    // this is empty — Reactions live in the DB.
    // Use Platform methods directly instead.

    // Keyed by user id — a user can hold at most one reaction at a time
    public Dictionary<Guid, ReactionType> Reactions { get; } = new();

    /// <summary>Sets the given user's reaction. Setting the same reaction again clears it
    /// (toggle off); setting a different reaction replaces it.</summary>
    public void SetReaction(Guid userId, ReactionType reaction)
    {
        if (Reactions.TryGetValue(userId, out var existing) && existing == reaction)
            Reactions.Remove(userId);
        else
            Reactions[userId] = reaction;
    }

}