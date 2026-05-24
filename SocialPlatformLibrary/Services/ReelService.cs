using System;
using System.Collections.Generic;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Services;

/// <summary>
/// Handles creation, retrieval, updates, and interactions for Reel posts.
/// </summary>
public class ReelService
{
    // Repository abstraction — swappable (memory, database, etc.)
    IReelRepo _repo;

    public ReelService(IReelRepo repo)
    {
        _repo = repo;
    }

    /// <summary>Creates a new reel after validating author and content.</summary>
    public Reel CreateReel(ReelDTO reel)
    {
        if (reel == null)
            throw new ArgumentNullException(nameof(reel));
        if (reel.Author == null || reel.Author.Id == Guid.Empty)
            throw new ArgumentException("Reel must have a valid author.", nameof(reel));
        if (string.IsNullOrWhiteSpace(reel.Content))
            throw new ArgumentException("Reel content cannot be empty.", nameof(reel));

        return _repo.CreateReel(reel);
    }

    /// <summary>
    /// Updates the content of an existing reel.
    /// Returns null if the reel does not exist.
    /// </summary>
    public Reel UpdateReelById(Guid id, string newContent)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Reel content cannot be empty.", nameof(newContent));

        // Return null early if reel not found
        var existing = _repo.GetReelById(id);
        if (existing == null)
            return null;

        return _repo.UpdateReelById(id, newContent);
    }

    public List<Reel> GetAllReels()
        => _repo.GetAllReels();

    public Reel GetReelById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));

        return _repo.GetReelById(id);
    }

    public bool RemoveReelById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));

        return _repo.RemoveReelById(id);
    }

    /// <summary>Toggles a like on a reel. Throws if the reel is not found.</summary>
    public void ToggleLikeReel(Guid reelId, Guid userId)
    {
        var reel = _repo.GetReelById(reelId);
        if (reel == null)
            throw new KeyNotFoundException($"Reel with id {reelId} not found.");
        _repo.ToggleLike(reelId, userId);
    }

    public HashSet<Guid> GetLikes(Guid reelId)
    {
        if (reelId == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(reelId));
        return _repo.GetLikes(reelId);
    }

}