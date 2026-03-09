using System;
using System.Collections.Generic;
using System.Text;
using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Interfaces;
using SocialNetworkingPlatform.Posts;

namespace SocialNetworkingPlatform.Services;

public class StoryService
{
    IStoryRepo _repo;

    public StoryService(IStoryRepo repo)
    {
        _repo = repo;
    }

    public Story CreateStory(StoryDTO story)
    {
        return _repo.CreateStory(story);
    }

    public Story UpdateStoryById(Guid id, string newContent)
    {
        return _repo.UpdateStoryById(id, newContent);
    }

    public List<Story> GetAllStories()
    {
        return _repo.GetAllStories();
    }

    public Story GetStoryById(Guid id)
    {
        return _repo.GetStoryById(id);
    }

    public bool RemoveStoryById(Guid id)
    {
        return _repo.RemoveStoryById(id);
    }

}
