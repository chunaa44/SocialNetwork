using SocialPlatformLibrary.Interfaces;
using System;
using System.Collections.Generic;

namespace SocialPlatformLibrary.Posts;

/// <summary>
/// A temporary post that expires after 24 hours. Supports reactions and view tracking.
/// </summary>
public class Story : Post, IReactable, IViewTrackable
{
    private static readonly TimeSpan DefaultDuration = TimeSpan.FromHours(24);

    // Calculated once at construction from the post timestamp
    public DateTime ExpiresAt { get; init; }

    // NOTE: Only accurate immediately after creation. Once fetched from SQLite,
    // this is empty — Views live in the DB.
    // Use Platform methods directly instead.

    // HashSet ensures each user is counted only once
    public HashSet<Guid> Viewers { get; } = new HashSet<Guid>();

    // ViewCount is derived from Viewers so it always stays in sync 
    public int ViewCount => Viewers.Count;

    // Keyed by user id — a user can hold at most one reaction at a time
    public Dictionary<Guid, ReactionType> Reactions { get; } = new();

    public Story()
    {
        ExpiresAt = Timestamp + DefaultDuration;
    }

    // Compares against the current system clock
    public bool IsExpired => DateTime.Now > ExpiresAt;


    /// <summary>Sets the given user's reaction. Setting the same reaction again clears it
    /// (toggle off); setting a different reaction replaces it. Throws if the story has expired.</summary>
    public void SetReaction(Guid userId, ReactionType reaction)
    {
        if (IsExpired)
            throw new InvalidOperationException("Cannot react to an expired story.");
        if (Reactions.TryGetValue(userId, out var existing) && existing == reaction)
            Reactions.Remove(userId);
        else
            Reactions[userId] = reaction;
    }

    /// <summary>
    /// Records a unique view. Returns true if this is a new viewer; 
    /// false if the user already viewed it or the story is expired.
    /// </summary>
    public bool AddView(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Invalid user id.", nameof(userId));

        // Do not count views on expired stories
        if (IsExpired)
            return false;

        // HashSet.Add returns false if the element already existed
        return Viewers.Add(userId);
    }
}