using SocialNetworkingPlatform.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkingPlatform.Posts;

public class Photo: Post, ILikable, ICommentable, IBookmarkable
{
    public required string PhotoUrl { get; set; }

    public int LikeCount { get; private set; }
    public List<string> Comments { get; } = new List<string>();
    public List<Guid> Bookmarks { get; } = new List<Guid>();

    public void Like() => LikeCount++;
    public void AddComment(string userName, string text)
    {
        Comments.Add($"[{userName}]: {text}");
    }
    public void Bookmark(Guid userId)
    {
        if (!Bookmarks.Contains(userId)) Bookmarks.Add(userId);
    }
}
