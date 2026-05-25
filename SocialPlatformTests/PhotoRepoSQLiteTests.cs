using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Repositories;
using System;

namespace SocialPlatformTests;

[TestClass]
public class PhotoRepoSQLiteTests : SqliteTestBase
{
    private PhotoRepoSQLite _repo = null!;
    private User _author = null!;

    [TestInitialize]
    public void Setup()
    {
        // SqliteTestBase.OpenDb() runs first (TestInitialize order: base then derived)
        _repo = new PhotoRepoSQLite(Connection);
        _author = new User { Name = "Alice", Email = "alice@example.com", Password = "password123" };
    }

    // make dto helper
    private PhotoDTO MakeDto(string content = "Sunset photo", string url = "https://img.example.com/1.jpg")
        => new PhotoDTO(_author, content, url);

    // ── CreatePhoto ──────────────────────────────────────────────────────────

    [TestMethod]
    public void CreatePhoto_ReturnsPhotoWithCorrectFields()
    {
        var photo = _repo.CreatePhoto(MakeDto());

        Assert.AreNotEqual(Guid.Empty, photo.Id);
        Assert.AreEqual(_author.Id, photo.AuthorId);
        Assert.AreEqual("Sunset photo", photo.Content);
        Assert.AreEqual("https://img.example.com/1.jpg", photo.PhotoUrl);
    }

    [TestMethod]
    public void CreatePhoto_IsPersisted_CanBeRetrieved()
    {
        var created = _repo.CreatePhoto(MakeDto());
        var found = _repo.GetPhotoById(created.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found.Id);
    }

    [TestMethod]
    public void CreatePhoto_TwoPhotos_HaveUniqueIds()
    {
        var p1 = _repo.CreatePhoto(MakeDto("First", "https://img.example.com/1.jpg"));
        var p2 = _repo.CreatePhoto(MakeDto("Second", "https://img.example.com/2.jpg"));

        Assert.AreNotEqual(p1.Id, p2.Id);
    }

    // ── GetAllPhotos ─────────────────────────────────────────────────────────

    [TestMethod]
    public void GetAllPhotos_EmptyTable_ReturnsEmptyList()
    {
        var result = _repo.GetAllPhotos();
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetAllPhotos_AfterInsertingTwo_ReturnsBoth()
    {
        _repo.CreatePhoto(MakeDto("A", "https://img.example.com/a.jpg"));
        _repo.CreatePhoto(MakeDto("B", "https://img.example.com/b.jpg"));

        Assert.AreEqual(2, _repo.GetAllPhotos().Count);
    }

    // ── GetPhotoById ─────────────────────────────────────────────────────────

    [TestMethod]
    public void GetPhotoById_ExistingId_ReturnsCorrectPhoto()
    {
        var created = _repo.CreatePhoto(MakeDto());
        var found = _repo.GetPhotoById(created.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found.Id);
        Assert.AreEqual(created.Content, found.Content);
        Assert.AreEqual(created.PhotoUrl, found.PhotoUrl);
        Assert.AreEqual(created.AuthorId, found.AuthorId);
    }

    [TestMethod]
    public void GetPhotoById_UnknownId_ReturnsNull()
    {
        var result = _repo.GetPhotoById(Guid.NewGuid());
        Assert.IsNull(result);
    }

    // ── UpdatePhotoById ──────────────────────────────────────────────────────

    [TestMethod]
    public void UpdatePhotoById_ExistingId_UpdatesContentAndUrl()
    {
        var photo = _repo.CreatePhoto(MakeDto());
        var updated = _repo.UpdatePhotoById(photo.Id, "New caption", "https://img.example.com/new.jpg");

        Assert.IsNotNull(updated);
        Assert.AreEqual("New caption", updated.Content);
        Assert.AreEqual("https://img.example.com/new.jpg", updated.PhotoUrl);
    }

    [TestMethod]
    public void UpdatePhotoById_ExistingId_ChangesArePersistedInDb()
    {
        var photo = _repo.CreatePhoto(MakeDto());
        _repo.UpdatePhotoById(photo.Id, "Updated", "https://img.example.com/updated.jpg");

        var fromDb = _repo.GetPhotoById(photo.Id);
        Assert.AreEqual("Updated", fromDb!.Content);
        Assert.AreEqual("https://img.example.com/updated.jpg", fromDb.PhotoUrl);
    }

    [TestMethod]
    public void UpdatePhotoById_UnknownId_ReturnsNull()
    {
        var result = _repo.UpdatePhotoById(Guid.NewGuid(), "X", "https://img.example.com/x.jpg");
        Assert.IsNull(result);
    }

    // ── RemovePhotoById ──────────────────────────────────────────────────────

    [TestMethod]
    public void RemovePhotoById_ExistingId_ReturnsTrueAndDeletesRow()
    {
        var photo = _repo.CreatePhoto(MakeDto());
        bool removed = _repo.RemovePhotoById(photo.Id);

        Assert.IsTrue(removed);
        Assert.IsNull(_repo.GetPhotoById(photo.Id));
    }

    [TestMethod]
    public void RemovePhotoById_UnknownId_ReturnsFalse()
    {
        bool result = _repo.RemovePhotoById(Guid.NewGuid());
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void RemovePhotoById_AlsoDeletesAssociatedLikes()
    {
        var photo = _repo.CreatePhoto(MakeDto());
        var userId = Guid.NewGuid();

        _repo.ToggleLike(photo.Id, userId);
        _repo.RemovePhotoById(photo.Id);

        // Likes table should be empty for that post
        var likes = _repo.GetLikes(photo.Id);
        Assert.AreEqual(0, likes.Count);
    }

    // ── ToggleLike ───────────────────────────────────────────────────────────

    [TestMethod]
    public void ToggleLike_FirstCall_AddsLikeToDb()
    {
        var photo = _repo.CreatePhoto(MakeDto());
        var userId = Guid.NewGuid();

        _repo.ToggleLike(photo.Id, userId);
        var likes = _repo.GetLikes(photo.Id);

        Assert.IsTrue(likes.Contains(userId));
    }

    [TestMethod]
    public void ToggleLike_SecondCall_RemovesLikeFromDb()
    {
        var photo = _repo.CreatePhoto(MakeDto());
        var userId = Guid.NewGuid();

        _repo.ToggleLike(photo.Id, userId); // add
        _repo.ToggleLike(photo.Id, userId); // remove
        var likes = _repo.GetLikes(photo.Id);

        Assert.IsFalse(likes.Contains(userId));
    }

    [TestMethod]
    public void ToggleLike_MultipleUsers_EachLikeStoredSeparately()
    {
        var photo = _repo.CreatePhoto(MakeDto());
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();

        _repo.ToggleLike(photo.Id, user1);
        _repo.ToggleLike(photo.Id, user2);
        var likes = _repo.GetLikes(photo.Id);

        Assert.AreEqual(2, likes.Count);
        Assert.IsTrue(likes.Contains(user1));
        Assert.IsTrue(likes.Contains(user2));
    }

    // ── GetLikes ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void GetLikes_NoLikes_ReturnsEmptySet()
    {
        var photo = _repo.CreatePhoto(MakeDto());
        var likes = _repo.GetLikes(photo.Id);
        Assert.AreEqual(0, likes.Count);
    }

    [TestMethod]
    public void GetLikes_AfterLiking_ReturnsCorrectUserId()
    {
        var photo = _repo.CreatePhoto(MakeDto());
        var userId = Guid.NewGuid();

        _repo.ToggleLike(photo.Id, userId);
        var likes = _repo.GetLikes(photo.Id);

        Assert.AreEqual(1, likes.Count);
        Assert.IsTrue(likes.Contains(userId));
    }
}