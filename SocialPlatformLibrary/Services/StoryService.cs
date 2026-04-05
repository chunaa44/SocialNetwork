using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Services;

public class StoryService
{
    IStoryRepo _repo;

    public StoryService(IStoryRepo repo)
    {
        _repo = repo;
    }

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

    public Story UpdateStoryById(Guid id, string newContent)
    {
        var existing = _repo.GetStoryById(id);
        if (existing == null)
            return null;

        // Do not allow updating expired stories
        if (existing.IsExpired)
            return null;

        return _repo.UpdateStoryById(id, newContent);
    }

    public List<Story> GetAllStories()
    {
        return _repo.GetAllStories();
    }

    // Return only non-expired stories
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

    // Register a view for a story; returns true when a new unique view was added.
    public bool AddView(Guid storyId, Guid userId)
    {
        var story = _repo.GetStoryById(storyId);
        if (story == null)
            throw new KeyNotFoundException($"Story with id {storyId} not found.");

        return story.AddView(userId);
    }

    // Like a story; throws when story missing or expired.
    public void LikeStory(Guid storyId, Guid userId)
    {
        var story = _repo.GetStoryById(storyId);
        if (story == null)
            throw new KeyNotFoundException($"Story with id {storyId} not found.");
        story.ToggleLike(userId);
    }

    // Remove all expired stories and return how many were removed.
    public int RemoveExpiredStories()
    {
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
