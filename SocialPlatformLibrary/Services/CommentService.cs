using System;
using System.Collections.Generic;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Services;

/// <summary>
/// Handles creation, retrieval, updates, and interactions for Comments.
/// Also keeps the parent post's comment list in sync.
/// </summary>
public class CommentService
{
    // Repository abstraction — swappable (memory, database, etc.)
    ICommentRepo _repo;

    public CommentService(ICommentRepo repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Creates a comment, saves it to the repo, and attaches it to the target post.
    /// Both the repo and the parent post's list are updated together.
    /// </summary>
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

        // Attach to the parent post so it shows up in post.Comments
        target.AddComment(created);
        return created;
    }

    /// <summary>
    /// Updates a comment's content.
    /// Returns null if the comment does not exist.
    /// </summary>
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

    /// <summary>
    /// Removes a comment from the repo and detaches it from the parent post.
    /// Both the repo and the parent post's list are updated together.
    /// </summary>
    public bool RemoveCommentById(Guid id, ICommentable target)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        // Remove from parent post's list first
        target.RemoveCommentById(id);
        return _repo.RemoveCommentById(id);
    }

    /// <summary>Toggles a like on a comment. Throws if the comment is not found.</summary>
    public void ToggleLikeComment(Guid commentId, Guid userId)
    {
        var comment = _repo.GetCommentById(commentId);
        if (comment == null)
            throw new KeyNotFoundException($"Comment with id {commentId} not found.");
        _repo.ToggleLike(commentId, userId);
    }

    public HashSet<Guid> GetLikes(Guid commentId)
    {
        if (commentId == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(commentId));
        return _repo.GetLikes(commentId);
    }
}
