using SocialPlatformLibrary.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Interfaces;

public interface IPhotoRepo
{
    public Photo CreatePhoto(PhotoDTO photo);
    public bool RemovePhotoById(Guid id);
    public Photo UpdatePhotoById(Guid id, string newContent, string newPhotoURL);
    public Photo GetPhotoById(Guid id);
    public List<Photo> GetAllPhotos();
}
