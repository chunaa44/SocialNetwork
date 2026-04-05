using SocialNetworkingPlatform.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkingPlatform.Posts;

public class Photo: Post, ILikable, ICommentable, IBookmarkable
{
    public required string PhotoUrl { get; set; }

    public HashSet<Guid> Likes { get; } = new HashSet<Guid>();
    public List<string> Comments { get; } = new List<string>();
    public List<Guid> Bookmarks { get; } = new List<Guid>();

    // Records like if not liked before
    // remove like if liked before
    public void ToggleLike(Guid userId)
    {
        if (!Likes.Contains(userId)) Likes.Add(userId);
        else Likes.Remove(userId);
    }

    public void AddComment(string userName, string text)
    {
        Comments.Add($"[{userName}]: {text}");
    }

    public void Bookmark(Guid userId)
    {
        if (!Bookmarks.Contains(userId)) Bookmarks.Add(userId);
    }
}
