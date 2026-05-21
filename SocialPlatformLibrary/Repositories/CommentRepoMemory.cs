using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialPlatformLibrary.Repositories;

public class CommentRepoMemory : ICommentRepo
{
    // simple in memory store
    List<Comment> comments = new List<Comment>();

    /// <summary>Creates and stores a new comment from the given DTO.</summary>
    public Comment CreateComment(CommentDTO comment)
    {
        var newComment = new Comment()
        {
            AuthorId = comment.Author.Id,
            Content = comment.Content,
            ParentId = comment.ParentId
        };

        comments.Add(newComment);
        return newComment;
    }

    public List<Comment> GetAllComments() => comments;

    /// <summary>Returns the comment with the given ID, or null if not found.</summary>
    public Comment GetCommentById(Guid id)
    {
        return comments.FirstOrDefault(c => c.Id == id);
    }

    /// <summary>Removes the comment with the given ID. 
    /// Returns true if removed, false if not found.</summary>
    public bool RemoveCommentById(Guid id)
    {
        int removed = comments.RemoveAll(c => c.Id == id);
        return removed > 0;
    }

    /// <summary>Updates the content of an existing comment. 
    /// Returns null if not found.</summary>
    public Comment UpdateCommentById(Guid id, string newContent)
    {
        var comment = GetCommentById(id);

        if (comment == null)
            return null;

        comment.Content = newContent;
        return comment;
    }
}
