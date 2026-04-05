using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;


namespace SocialPlatformLibrary.Repositories;


public class PhotoRepoMemory : IPhotoRepo
{
    List<Photo> photos = new List<Photo>();

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

    public Photo GetPhotoById(Guid id)
    {
        // Search the list for a photo with a matching ID
        return photos.FirstOrDefault(p => p.Id == id);
    }

    public bool RemovePhotoById(Guid id)
    {
        // Find how many items were removed. If > 0, return true.
        int removed = photos.RemoveAll(p => p.Id == id);
        return removed > 0;
    }

    public Photo UpdatePhotoById(Guid id, string newContent, string newPhotoURL)
    {
        var photo = GetPhotoById(id);

        if (photo == null)
            return null;

        photo.Content = newContent;
        photo.PhotoUrl = newPhotoURL;
        return photo;
    }
}