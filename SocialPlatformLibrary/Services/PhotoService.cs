using System;
using System.Collections.Generic;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Services;

/// <summary>
/// Handles creation, retrieval, updates, and interactions for Photo posts.
/// </summary>
public class PhotoService
{
    // Repository abstraction — swappable (memory, database, etc.)
    IPhotoRepo _repo;

    public PhotoService(IPhotoRepo repo)
    {
        _repo = repo;
    }

    /// <summary>Creates a new photo after validating author, content, and URL.</summary>
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

    /// <summary>
    /// Updates content and URL of an existing photo.
    /// Returns null if the photo does not exist.
    /// </summary>
    public Photo UpdatePhotoById(Guid id, string newContent, string newPhotoURL)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Photo content cannot be empty.", nameof(newContent));
        if (string.IsNullOrWhiteSpace(newPhotoURL))
            throw new ArgumentException("Photo URL cannot be empty.", nameof(newPhotoURL));

        // Return null early if photo not found 
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

    /// <summary>Toggles a like on a photo. Throws if the photo is not found.</summary>
    public void ToggleLikePhoto(Guid photoId, Guid userId)
    {
        var photo = _repo.GetPhotoById(photoId);
        if (photo == null)
            throw new KeyNotFoundException($"Photo with id {photoId} not found.");
        _repo.ToggleLike(photoId, userId);
    }

    public HashSet<Guid> GetLikes(Guid photoId)
    {
        if (photoId == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(photoId));
        return _repo.GetLikes(photoId);
    }

    /// <summary>Toggles a bookmark on a photo. Throws if the photo is not found.</summary>
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