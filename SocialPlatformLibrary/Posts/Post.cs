using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkingPlatform.Posts;

public abstract class Post 
{
    public Guid Id { get; } = Guid.NewGuid();
    public required Guid AuthorId { get; init; }
    public required string Content { get; set; }
    public DateTime Timestamp { get; } = DateTime.Now;
}
