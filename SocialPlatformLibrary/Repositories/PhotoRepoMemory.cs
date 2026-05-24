using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;


namespace SocialPlatformLibrary.Repositories;


public class PhotoRepoMemory : IPhotoRepo
{
    // simple in memory store
    List<Photo> photos = new List<Photo>();

    /// <summary>Creates and stores a new photo from the given DTO.</summary>
    public Photo CreatePhoto(PhotoDTO photo)
    {
        var newPhoto = new Photo()
        {
            AuthorId = photo.Author.Id,
            Content = photo.Content,
            PhotoUrl = photo.PhotoUrl
        };

        photos.Add(newPhoto);
        return newPhoto;
    }

    public List<Photo> GetAllPhotos()
    {
        return photos;
    }

    /// <summary>Returns the photo with the given ID, or null if not found.</summary>
    public Photo GetPhotoById(Guid id)
    {
        // Search the list for a photo with a matching ID
        return photos.FirstOrDefault(p => p.Id == id);
    }

    /// <summary>Removes the photo with the given ID. 
    /// Returns true if removed, false if not found.</summary>
    public bool RemovePhotoById(Guid id)
    {
        // Find how many items were removed. If > 0, return true.
        int removed = photos.RemoveAll(p => p.Id == id);
        return removed > 0;
    }

    /// <summary>Updates content and URL of an existing photo.
    /// Returns null if not found.</summary>
    public Photo UpdatePhotoById(Guid id, string newContent, string newPhotoURL)
    {
        var photo = GetPhotoById(id);

        if (photo == null)
            return null;

        photo.Content = newContent;
        photo.PhotoUrl = newPhotoURL;
        return photo;
    }

    public void ToggleLike(Guid photoId, Guid userId)
    {
        var photo = GetPhotoById(photoId);
        if (photo == null) return;
        photo.ToggleLike(userId);
    }

    public HashSet<Guid> GetLikes(Guid photoId)
    {
        var photo = GetPhotoById(photoId);
        if (photo == null) return new HashSet<Guid>();
        return photo.Likes;
    }
}