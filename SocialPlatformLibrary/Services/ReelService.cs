using System;
using System.Collections.Generic;
using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Interfaces;
using SocialNetworkingPlatform.Posts;

namespace SocialNetworkingPlatform.Services;

public class ReelService
{
    IReelRepo _repo;

    public ReelService(IReelRepo repo)
    {
        _repo = repo;
    }

    public Reel CreateReel(ReelDTO reel)
    {
        if (reel == null)
            throw new ArgumentNullException(nameof(reel));
        if (reel.author == null || reel.author.Id == Guid.Empty)
            throw new ArgumentException("Reel must have a valid author.", nameof(reel));
        if (string.IsNullOrWhiteSpace(reel.content))
            throw new ArgumentException("Reel content cannot be empty.", nameof(reel));

        return _repo.CreateReel(reel);
    }

    public Reel UpdateReelById(Guid id, string newContent)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Reel content cannot be empty.", nameof(newContent));

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

    // Like a reel; throws when reel missing.
    public void LikeReel(Guid reelId, Guid userId)
    {
        var reel = _repo.GetReelById(reelId);
        if (reel == null)
            throw new KeyNotFoundException($"Reel with id {reelId} not found.");
        reel.ToggleLike(userId);
    }

    // Add a comment to a reel; validates simple inputs.
    public void AddComment(Guid reelId, string userName, string text)
    {
        var reel = _repo.GetReelById(reelId);
        if (reel == null)
            throw new KeyNotFoundException($"Reel with id {reelId} not found.");

        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User name is required.", nameof(userName));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Comment text is required.", nameof(text));

        reel.AddComment(userName, text);
    }
}