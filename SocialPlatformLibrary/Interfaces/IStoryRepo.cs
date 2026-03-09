using SocialNetworkingPlatform.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using SocialNetworkingPlatform.Posts;

namespace SocialNetworkingPlatform.Interfaces;

public interface IStoryRepo
{
    public Story CreateStory(StoryDTO story);
    public bool RemoveStoryById(Guid id);
    public Story UpdateStoryById(Guid id, string newContent);
    public Story GetStoryById(Guid id);
    public List<Story> GetAllStories();
}
