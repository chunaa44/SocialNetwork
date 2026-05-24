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

    void ToggleLike(Guid id, Guid userId);

    HashSet<Guid> GetLikes(Guid id);
}
