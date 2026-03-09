using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Interfaces;
using SocialNetworkingPlatform.Posts;

namespace SocialNetworkingPlatform.Services;

public class PhotoService
{
    IPhotoRepo _repo;

    public PhotoService(IPhotoRepo repo)
    {
        _repo = repo;
    }

    public Photo CreatePhoto(PhotoDTO photo) => _repo.CreatePhoto(photo);

    public Photo UpdatePhotoById(Guid id, string newContent, string newPhotoURL)
        => _repo.UpdatePhotoById(id, newContent, newPhotoURL);

    public List<Photo> GetAllPhotos() => _repo.GetAllPhotos();

    public Photo GetPhotoById(Guid id) => _repo.GetPhotoById(id);

    public bool RemovePhotoById(Guid id) => _repo.RemovePhotoById(id);
}