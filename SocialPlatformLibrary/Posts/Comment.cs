using SocialPlatformLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SocialPlatformLibrary.Posts;

public class Comment: Post, ILikable
{
    public required Guid ParentId{ get; init; }
    public HashSet<Guid> Likes { get; } = new HashSet<Guid>();
    public void ToggleLike(Guid userId)
    {
        if (!Likes.Contains(userId)) Likes.Add(userId);
        else Likes.Remove(userId);
    }

}
