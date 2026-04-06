using System;
using System.Collections.Generic;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Services;

public class CommentService
{
    ICommentRepo _repo;

    public CommentService(ICommentRepo repo)
    {
        _repo = repo;
    }

    public Comment CreateComment(CommentDTO comment, ICommentable target)
    {
        if (comment == null)
            throw new ArgumentNullException(nameof(comment));
        if (comment.Author == null || comment.Author.Id == Guid.Empty)
            throw new ArgumentException("Comment must have a valid author.", nameof(comment));
        if (string.IsNullOrWhiteSpace(comment.Content))
            throw new ArgumentException("Comment content cannot be empty.", nameof(comment));
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        var created = _repo.CreateComment(comment);
        target.AddComment(created);
        return created;
    }

    public Comment UpdateCommentById(Guid id, string newContent)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Comment content cannot be empty.", nameof(newContent));

        var existing = _repo.GetCommentById(id);
        if (existing == null)
            return null;

        return _repo.UpdateCommentById(id, newContent);
    }

    public List<Comment> GetAllComments() => _repo.GetAllComments();

    public Comment GetCommentById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));

        return _repo.GetCommentById(id);
    }

    public bool RemoveCommentById(Guid id, ICommentable target)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        target.RemoveCommentById(id);
        return _repo.RemoveCommentById(id);
    }

    public void ToggleLikeComment(Guid commentId, Guid userId)
    {
        var comment = _repo.GetCommentById(commentId);
        if (comment == null)
            throw new KeyNotFoundException($"Comment with id {commentId} not found.");
        comment.ToggleLike(userId);
    }
}
