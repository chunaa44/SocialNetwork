using SocialPlatformLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Interfaces;

public interface IStoryRepo
{
    /// <summary>Creates a new story and returns the created instance.</summary>
    public Story CreateStory(StoryDTO story);

    /// <summary>Removes the story with the given ID. Returns true if removed.</summary>
    public bool RemoveStoryById(Guid id);

    /// <summary>Updates content of an existing story. 
    /// Returns null if not found.</summary>
    public Story UpdateStoryById(Guid id, string newContent);

    /// <summary>Returns the story with the given ID, or null if not found.</summary>
    public Story GetStoryById(Guid id);

    /// <summary>Returns all stories in the store, including expired ones.</summary>
    public List<Story> GetAllStories();

    /// <summary>Adds a like if the user hasn't liked it yet; removes it if they already have.</summary>
    void ToggleLike(Guid id, Guid userId);

    /// <summary>Returns the set of user IDs who have liked this photo. </summary>
    HashSet<Guid> GetLikes(Guid id);

    /// <summary>Records a view for the given user. Returns true if this was a new unique view.</summary>
    bool AddView(Guid id, Guid userId);

    /// <summary>Returns the set of user IDs who have viewed this story.</summary>
    HashSet<Guid> GetViewers(Guid id);
}
