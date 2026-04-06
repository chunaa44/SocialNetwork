using System;
using System.Collections.Generic;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Services;

public class PhotoService
{
    IPhotoRepo _repo;

    public PhotoService(IPhotoRepo repo)
    {
        _repo = repo;
    }

    public Photo CreatePhoto(PhotoDTO photo)
    {
        if (photo == null)
            throw new ArgumentNullException(nameof(photo));
        if (photo.Author == null || photo.Author.Id == Guid.Empty)
            throw new ArgumentException("Photo must have a valid author.", nameof(photo));
        if (string.IsNullOrWhiteSpace(photo.Content))
            throw new ArgumentException("Photo content cannot be empty.", nameof(photo));
        if (string.IsNullOrWhiteSpace(photo.PhotoUrl))
            throw new ArgumentException("Photo URL cannot be empty.", nameof(photo));

        return _repo.CreatePhoto(photo);
    }

    public Photo UpdatePhotoById(Guid id, string newContent, string newPhotoURL)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Photo content cannot be empty.", nameof(newContent));
        if (string.IsNullOrWhiteSpace(newPhotoURL))
            throw new ArgumentException("Photo URL cannot be empty.", nameof(newPhotoURL));

        var existing = _repo.GetPhotoById(id);
        if (existing == null)
            return null;

        return _repo.UpdatePhotoById(id, newContent, newPhotoURL);
    }

    public List<Photo> GetAllPhotos() => _repo.GetAllPhotos();

    public Photo GetPhotoById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));

        return _repo.GetPhotoById(id);
    }

    public bool RemovePhotoById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));

        return _repo.RemovePhotoById(id);
    }

    // Like a photo; throws when photo missing.
    public void ToggleLikePhoto(Guid photoId, Guid userId)
    {
        var photo = _repo.GetPhotoById(photoId);
        if (photo == null)
            throw new KeyNotFoundException($"Photo with id {photoId} not found.");
        photo.ToggleLike(userId);
    }

    // Bookmark a photo for a user.
    public void ToggleBookmarkPhoto(Guid photoId, Guid userId)
    {
        var photo = _repo.GetPhotoById(photoId);
        if (photo == null)
            throw new KeyNotFoundException($"Photo with id {photoId} not found.");
        if (userId == Guid.Empty)
            throw new ArgumentException("User id must be provided.", nameof(userId));

        photo.ToggleBookmark(userId);
    }
}