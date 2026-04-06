using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Posts;

public class Reel: Post, ILikable, ICommentable
{
    public HashSet<Guid> Likes { get; } = new HashSet<Guid>();
    public List<Comment> Comments { get; } = new List<Comment>();

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
}
