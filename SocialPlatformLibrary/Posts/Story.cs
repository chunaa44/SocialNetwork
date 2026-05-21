using SocialPlatformLibrary.Interfaces;
using System;
using System.Collections.Generic;

namespace SocialPlatformLibrary.Posts;

/// <summary>
/// A temporary post that expires after 24 hours. Supports likes and view tracking.
/// </summary>
public class Story : Post, ILikable, IViewTrackable
{
    // All stories expire after 24 hours by default
    private static readonly TimeSpan DefaultDuration = TimeSpan.FromHours(24);

    // Calculated once at construction from the post timestamp
    public DateTime ExpiresAt { get; private set; }

    // HashSet ensures each user is counted only once
    public HashSet<Guid> Viewers { get; } = new HashSet<Guid>();

    // ViewCount is derived from Viewers so it always stays in sync 
    public int ViewCount => Viewers.Count;

    // HashSet prevents duplicate likes from the same user
    public HashSet<Guid> Likes { get; } = new HashSet<Guid>();

    public Story()
    {
        ExpiresAt = Timestamp + DefaultDuration;
    }

    // Compares against the current system clock
    public bool IsExpired => DateTime.Now > ExpiresAt;


    /// <summary>Toggles a like. Throws if the story has expired.</summary>
    public void ToggleLike(Guid userId)
    {
        if (IsExpired)
            throw new InvalidOperationException("Cannot like an expired story.");
        if (!Likes.Contains(userId)) Likes.Add(userId);
        else Likes.Remove(userId);
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
