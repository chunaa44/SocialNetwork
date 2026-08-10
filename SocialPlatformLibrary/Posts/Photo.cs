using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Posts;

/// <summary>
/// A photo post. Supports reacts, comments, and bookmarks.
/// </summary>
public class Photo: Post, IReactable, ICommentable, IBookmarkable
{
    public required string PhotoUrl { get; set; }

    // NOTE: Only accurate immediately after creation. Once fetched from SQLite (PhotoRepoSQLite),
    // this is empty — Reacts, Bookmarks and Comments live in the DB.
    // Use Platform methods instead.

    // Keyed by user id — a user can hold at most one reaction at a time
    public Dictionary<Guid, ReactionType> Reactions { get; } = new Dictionary<Guid, ReactionType>();

    public List<Comment> Comments { get; } = new List<Comment>();

    // HashSet prevents a user from bookmarking the same photo twice
    public HashSet<Guid> Bookmarks { get; } = new HashSet<Guid>();

    /// <summary>Sets the given user's reaction. Setting the same reaction again clears it
    /// (toggle off); setting a different reaction replaces it.</summary>
    public void SetReaction(Guid userId, ReactionType reaction)
    {
        if (Reactions.TryGetValue(userId, out var existing) && existing == reaction)
            Reactions.Remove(userId);
        else
            Reactions[userId] = reaction;
    }


    public void AddComment(Comment comment)
    { 
        Comments.Add(comment);
    }

    public void RemoveCommentById(Guid commentId)
    {
        // Find the comment by ID and remove it if it exists
        var comment = Comments.FirstOrDefault(c => c.Id == commentId);
        if(comment != null)
        {
            Comments.Remove(comment);
        }
    }

    /// <summary>Adds a bookmark if not bookmarked; removes it if already bookmarked.</summary>
    public void ToggleBookmark(Guid userId)
    {
        if (!Bookmarks.Contains(userId)) Bookmarks.Add(userId);
        else Bookmarks.Remove(userId);
    }
}
