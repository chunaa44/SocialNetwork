using SocialNetworkingPlatform.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkingPlatform.Posts;

public class Reel: Post, ILikable, ICommentable
{
    public HashSet<Guid> Likes { get; } = new HashSet<Guid>();
    public List<string> Comments { get; } = new List<string>();

    public void ToggleLike(Guid userId)
    {
        if (!Likes.Contains(userId)) Likes.Add(userId);
        else Likes.Remove(userId);
    }

    public void AddComment(string userName, string text)
    {
        Comments.Add($"[{userName}]: {text}");
    }
}
