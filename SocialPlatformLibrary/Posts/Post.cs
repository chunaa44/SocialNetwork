using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Posts;

/// <summary>
/// Abstract base class for all post types (Photo, Reel, Story, Comment).
/// Holds the common properties every post must have.
/// </summary>
public abstract class Post 
{
    // Unique identifier assigned at creation, never changes
    public Guid Id { get; init; } = Guid.NewGuid();

    // ID of the user who created this post
    public required Guid AuthorId { get; init; }

    public required string Content { get; set; }

    // Recorded once at creation, never modified
    public DateTime Timestamp { get; init; } = DateTime.Now;
}
