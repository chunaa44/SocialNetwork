using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialPlatformLibrary.Repositories;

public class CommentRepoMemory : ICommentRepo
{
    List<Comment> comments = new List<Comment>();

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

    public Comment GetCommentById(Guid id)
    {
        return comments.FirstOrDefault(c => c.Id == id);
    }

    public bool RemoveCommentById(Guid id)
    {
        int removed = comments.RemoveAll(c => c.Id == id);
        return removed > 0;
    }

    public Comment UpdateCommentById(Guid id, string newContent)
    {
        var comment = GetCommentById(id);

        if (comment == null)
            return null;

        comment.Content = newContent;
        return comment;
    }
}
