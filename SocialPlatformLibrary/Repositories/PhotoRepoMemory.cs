using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Interfaces;
using SocialNetworkingPlatform.Posts;
using System.Linq;
using SocialNetworkingPlatform.Posts;

namespace SocialNetworkingPlatform.Repositories;


public class PhotoRepoMemory : IPhotoRepo
{
    List<Photo> photos = new List<Photo>();

    public Photo CreatePhoto(PhotoDTO photo)
    {
        var newPhoto = new Photo()
        {
            AuthorId = photo.author.Id,
            Content = photo.content,
            PhotoUrl = photo.photoUrl
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