using SocialPlatformLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Interfaces;

public interface IPhotoRepo
{
    /// <summary>Creates a new photo and returns the created instance.</summary>
    public Photo CreatePhoto(PhotoDTO photo);

    /// <summary>Removes the photo with the given ID. Returns true if removed.</summary>
    public bool RemovePhotoById(Guid id);

    /// <summary>Updates content and URL of an existing photo.
    /// Returns null if not found.</summary>
    public Photo UpdatePhotoById(Guid id, string newContent, string newPhotoURL);

    /// <summary>Returns the photo with the given ID, or null if not found.</summary>
    public Photo GetPhotoById(Guid id);

    /// <summary>Returns all photos in the store.</summary>
    public List<Photo> GetAllPhotos();

    /// <summary>Sets a user's reaction. Setting the same reaction the user already has
    /// removes it (toggle off); setting a different reaction replaces it.</summary>
    void SetReaction(Guid id, Guid userId, ReactionType reaction);

    /// <summary>Returns all reactions on this photo, keyed by user id.</summary>
    Dictionary<Guid, ReactionType> GetReactions(Guid id);

    /// <summary>Adds a bookmark if the user hasn't bookmarked it yet; removes it if they already have.</summary>
    void ToggleBookmark(Guid id, Guid userId);

    /// <summary>Returns the set of user IDs who have bookmarked this photo.</summary>
    HashSet<Guid> GetBookmarks(Guid id);
}