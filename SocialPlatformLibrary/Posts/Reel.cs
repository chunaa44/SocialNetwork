using SocialNetworkingPlatform.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkingPlatform.Posts;

public class Reel: Post, ILikable, ICommentable
{
    public int LikeCount { get; private set; }
    public List<string> Comments { get; } = new List<string>();

    public void Like() => LikeCount++;
    public void AddComment(string userName, string text)
    {
        Comments.Add($"[{userName}]: {text}");
    }
}
