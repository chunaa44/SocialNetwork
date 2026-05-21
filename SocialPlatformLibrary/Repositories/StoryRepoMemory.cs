using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Repositories;

public class StoryRepoMemory : IStoryRepo
{
    // simple in memory store
    List<Story> stories = new List<Story>();

    /// <summary>Creates and stores a new story from the given DTO.</summary>
    public Story CreateStory(StoryDTO story)
    {
        var newStory = new Story()
        {
            AuthorId = story.Author.Id,
            Content = story.Content
        };
        stories.Add(newStory);
        return newStory;
    }

    public List<Story> GetAllStories()
    {
        return stories;
    }

    /// <summary>Returns the story with the given ID, or null if not found.</summary>
    public Story GetStoryById(Guid id)
    {
        // Search the list for a story with a matching ID
        return stories.FirstOrDefault(s => s.Id == id);
    }

    /// <summary>Removes the story with the given ID. 
    /// Returns true if removed, false if not found.</summary>
    public bool RemoveStoryById(Guid id)
    {
        // Find how many items were removed. If > 0, return true.
        int removedCount = stories.RemoveAll(s => s.Id == id);
        return removedCount > 0;
    }

    /// <summary>Updates the content of an existing story. 
    /// Returns null if not found.</summary>
    public Story UpdateStoryById(Guid id, string newContent)
    {
        // 1. Find the existing story
        var existingStory = GetStoryById(id);

        if (existingStory == null)
            return null;

        // 2. Update the properties
        existingStory.Content = newContent;

        // 3. Return the updated object
        return existingStory;
    }
}
