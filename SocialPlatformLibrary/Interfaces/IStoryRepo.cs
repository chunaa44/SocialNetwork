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

    /// <summary>Sets a user's reaction. Setting the same reaction the user already has
    /// removes it (toggle off); setting a different reaction replaces it.</summary>
    void SetReaction(Guid id, Guid userId, ReactionType reaction);

    /// <summary>Returns all reactions on this story, keyed by user id.</summary>
    Dictionary<Guid, ReactionType> GetReactions(Guid id);

    /// <summary>Records a view for the given user. Returns true if this was a new unique view.</summary>
    bool AddView(Guid id, Guid userId);

    /// <summary>Returns the set of user IDs who have viewed this story.</summary>
    HashSet<Guid> GetViewers(Guid id);
}