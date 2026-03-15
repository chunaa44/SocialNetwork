using SocialNetworkingPlatform.Interfaces;
using System;
using System.Collections.Generic;

namespace SocialNetworkingPlatform.Posts;

public class Story : Post, ILikable, IViewTrackable
{
    private static readonly TimeSpan DefaultDuration = TimeSpan.FromHours(24);

    // When the story will expire (set at creation).
    public DateTime ExpiresAt { get; private set; }

    // Viewers tracked to avoid duplicate view counts.
    public HashSet<Guid> Viewers { get; } = new HashSet<Guid>();

    public int ViewCount => Viewers.Count;

    public int LikeCount { get; private set; }

    public Story()
    {
        ExpiresAt = Timestamp + DefaultDuration;
    }

    // Whether story is expired (reads system clock).
    public bool IsExpired => DateTime.Now > ExpiresAt;

    // Like a story only when active.
    public void Like()
    {
        if (IsExpired)
            throw new InvalidOperationException("Cannot like an expired story.");
        LikeCount++;
    }

    // Add a view for a unique user; returns true when a new view was registered.
    public bool AddView(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Invalid user id.", nameof(userId));
        if (IsExpired)
            return false;

        return Viewers.Add(userId);
    }
}
