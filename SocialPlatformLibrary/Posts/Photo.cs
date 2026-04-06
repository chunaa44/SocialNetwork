using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Posts;

public class Photo: Post, ILikable, ICommentable, IBookmarkable
{
    public required string PhotoUrl { get; set; }

    public HashSet<Guid> Likes { get; } = new HashSet<Guid>();
    public List<Comment> Comments { get; } = new List<Comment>();
    public HashSet<Guid> Bookmarks { get; } = new HashSet<Guid>();

    // Records like if not liked before
    // remove like if liked before
    public void ToggleLike(Guid userId)
    {
        if (!Likes.Contains(userId)) Likes.Add(userId);
        else Likes.Remove(userId);
    }

    public void AddComment(Comment comment)
    { 
        Comments.Add(comment);
    }

    public void RemoveCommentById(Guid commentId)
    {
        var comment = Comments.FirstOrDefault(c => c.Id == commentId);
        if(comment != null)
        {
            Comments.Remove(comment);
        }
    }

    public void ToggleBookmark(Guid userId)
    {
        if (!Bookmarks.Contains(userId)) Bookmarks.Add(userId);
        else Bookmarks.Remove(userId);
    }
}
