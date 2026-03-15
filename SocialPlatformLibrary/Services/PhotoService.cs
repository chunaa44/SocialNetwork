using System;
using System.Collections.Generic;
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

    public Photo CreatePhoto(PhotoDTO photo)
    {
        if (photo == null)
            throw new ArgumentNullException(nameof(photo));
        if (photo.author == null || photo.author.Id == Guid.Empty)
            throw new ArgumentException("Photo must have a valid author.", nameof(photo));
        if (string.IsNullOrWhiteSpace(photo.content))
            throw new ArgumentException("Photo content cannot be empty.", nameof(photo));
        if (string.IsNullOrWhiteSpace(photo.photoUrl))
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
    public void LikePhoto(Guid photoId)
    {
        var photo = _repo.GetPhotoById(photoId);
        if (photo == null)
            throw new KeyNotFoundException($"Photo with id {photoId} not found.");
        photo.Like();
    }

    public void AddComment(Guid photoId, string userName, string text)
    {
        var photo = _repo.GetPhotoById(photoId);
        if (photo == null)
            throw new KeyNotFoundException($"Photo with id {photoId} not found.");

        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User name is required.", nameof(userName));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Comment text is required.", nameof(text));

        photo.AddComment(userName, text);
    }

    // Bookmark a photo for a user.
    public void BookmarkPhoto(Guid photoId, Guid userId)
    {
        var photo = _repo.GetPhotoById(photoId);
        if (photo == null)
            throw new KeyNotFoundException($"Photo with id {photoId} not found.");
        if (userId == Guid.Empty)
            throw new ArgumentException("User id must be provided.", nameof(userId));

        photo.Bookmark(userId);
    }
}