using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Posts;

public abstract class Post 
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid AuthorId { get; init; }
    public required string Content { get; set; }
    public DateTime Timestamp { get; init; } = DateTime.Now;
}
