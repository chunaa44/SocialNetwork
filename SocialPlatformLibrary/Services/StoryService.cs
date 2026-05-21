using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Services;

/// <summary>
/// Handles creation, retrieval, updates, and interactions for Story posts.
/// Includes expiry management unique to stories.
/// </summary>
public class StoryService
{
    // Repository abstraction — swappable (memory, database, etc.)
    IStoryRepo _repo;

    public StoryService(IStoryRepo repo)
    {
        _repo = repo;
    }

    /// <summary>Creates a new story after validating author and content.</summary>
    public Story CreateStory(StoryDTO story)
    {
        if (story == null)
            throw new ArgumentNullException(nameof(story));
        if (story.Author == null || story.Author.Id == Guid.Empty)
            throw new ArgumentException("Story must have a valid author.", nameof(story));
        if (string.IsNullOrWhiteSpace(story.Content))
            throw new ArgumentException("Story content cannot be empty.", nameof(story));

        return _repo.CreateStory(story);
    }

    /// <summary>
    /// Updates an existing story's content.
    /// Returns null if not found or already expired.
    /// </summary>
    public Story UpdateStoryById(Guid id, string newContent)
    {
        var existing = _repo.GetStoryById(id);
        if (existing == null)
            return null;

        // Expired stories should not be editable
        if (existing.IsExpired)
            return null;

        return _repo.UpdateStoryById(id, newContent);
    }

    public List<Story> GetAllStories()
    {
        return _repo.GetAllStories();
    }

    /// <summary>Returns only stories that have not yet expired.</summary>
    public List<Story> GetActiveStories()
    {
        return _repo.GetAllStories().Where(s => !s.IsExpired).ToList();
    }

    public Story GetStoryById(Guid id)
    {
        return _repo.GetStoryById(id);
    }

    public bool RemoveStoryById(Guid id)
    {
        return _repo.RemoveStoryById(id);
    }

    /// <summary>Records a view for a user on a story. Returns true if it was a new unique view.</summary>
    public bool AddView(Guid storyId, Guid userId)
    {
        var story = _repo.GetStoryById(storyId);
        if (story == null)
            throw new KeyNotFoundException($"Story with id {storyId} not found.");

        return story.AddView(userId);
    }

    /// <summary>Toggles a like on a story. Throws if not found or expired.</summary>
    public void ToggleLikeStory(Guid storyId, Guid userId)
    {
        var story = _repo.GetStoryById(storyId);
        if (story == null)
            throw new KeyNotFoundException($"Story with id {storyId} not found.");
        story.ToggleLike(userId);
    }

    /// <summary>
    /// Deletes all expired stories from the repository.
    /// Returns the number of stories removed.
    /// </summary>
    public int RemoveExpiredStories()
    {
        // Collect IDs first to avoid modifying the collection while iterating
        var expiredIds = _repo.GetAllStories()
                              .Where(s => s.IsExpired)
                              .Select(s => s.Id)
                              .ToList();

        var removed = 0;
        foreach (var id in expiredIds)
        {
            if (_repo.RemoveStoryById(id))
                removed++;
        }
        return removed;
    }
}
