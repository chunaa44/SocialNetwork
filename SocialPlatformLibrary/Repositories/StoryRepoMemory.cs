using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SocialNetworkingPlatform.Posts;

namespace SocialNetworkingPlatform.Repositories;

public class StoryRepoMemory : IStoryRepo
{
    List<Story> stories = new List<Story>();

    public Story CreateStory(StoryDTO story)
    {
        var newStory = new Story()
        {
            AuthorId = story.author.Id,
            Content = story.content
        };
        stories.Add(newStory);
        return newStory;
    }

    public List<Story> GetAllStories()
    {
        return stories;
    }

    public Story GetStoryById(Guid id)
    {
        // Search the list for a story with a matching ID
        return stories.FirstOrDefault(s => s.Id == id);
    }

    public bool RemoveStoryById(Guid id)
    {
        // Find how many items were removed. If > 0, return true.
        int removedCount = stories.RemoveAll(s => s.Id == id);
        return removedCount > 0;
    }

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
