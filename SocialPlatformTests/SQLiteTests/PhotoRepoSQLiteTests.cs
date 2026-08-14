using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Posts;
using SocialPlatformLibrary.Repositories;
using SocialPlatformTests.SQLiteTests;

namespace SocialPlatformTests;

[TestClass]
public class PhotoRepoSQLiteTests : SqliteTestBase
{
    private PhotoRepoSQLite _repo = null!;
    private User _author = null!;

    [TestInitialize]
    public void Init()
    {
        _repo = new PhotoRepoSQLite(Connection);
        var userRepo = new UserRepoSQLite(Connection);
        _author = userRepo.CreateUser(new UserDTO("Alice", "alice@example.com", "password123"));
    }

    private Photo NewPhoto(string content = "sunset", string url = "https://example.com/sunset.jpg")
        => _repo.CreatePhoto(new PhotoDTO(_author, content, url));

    [TestMethod]
    public void CreatePhoto_ReturnsPhotoWithGeneratedId()
    {
        var photo = NewPhoto();

        Assert.AreNotEqual(Guid.Empty, photo.Id);
        Assert.AreEqual(_author.Id, photo.AuthorId);
        Assert.AreEqual("sunset", photo.Content);
        Assert.AreEqual("https://example.com/sunset.jpg", photo.PhotoUrl);
    }

    [TestMethod]
    public void GetPhotoById_ExistingPhoto_ReturnsPhoto()
    {
        var created = NewPhoto();

        var found = _repo.GetPhotoById(created.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found!.Id);
    }

    [TestMethod]
    public void GetPhotoById_NonExistentPhoto_ReturnsNull()
    {
        var found = _repo.GetPhotoById(Guid.NewGuid());

        Assert.IsNull(found);
    }

    [TestMethod]
    public void GetAllPhotos_ReturnsAllCreatedPhotos()
    {
        NewPhoto("first", "https://example.com/1.jpg");
        NewPhoto("second", "https://example.com/2.jpg");

        var all = _repo.GetAllPhotos();

        Assert.AreEqual(2, all.Count);
    }

    [TestMethod]
    public void UpdatePhotoById_ExistingPhoto_UpdatesContentAndUrl()
    {
        var created = NewPhoto();

        var updated = _repo.UpdatePhotoById(created.Id, "new content", "https://example.com/new.jpg");

        Assert.IsNotNull(updated);
        Assert.AreEqual("new content", updated!.Content);
        Assert.AreEqual("https://example.com/new.jpg", updated.PhotoUrl);
    }

    [TestMethod]
    public void UpdatePhotoById_NonExistentPhoto_ReturnsNull()
    {
        var updated = _repo.UpdatePhotoById(Guid.NewGuid(), "content", "https://example.com/x.jpg");

        Assert.IsNull(updated);
    }

    [TestMethod]
    public void RemovePhotoById_ExistingPhoto_RemovesAndReturnsTrue()
    {
        var created = NewPhoto();

        var removed = _repo.RemovePhotoById(created.Id);

        Assert.IsTrue(removed);
        Assert.IsNull(_repo.GetPhotoById(created.Id));
    }

    [TestMethod]
    public void RemovePhotoById_NonExistentPhoto_ReturnsFalse()
    {
        var removed = _repo.RemovePhotoById(Guid.NewGuid());

        Assert.IsFalse(removed);
    }

    [TestMethod]
    public void SetReaction_NewReaction_InsertsReaction()
    {
        var photo = NewPhoto();

        _repo.SetReaction(photo.Id, _author.Id, ReactionType.Like);

        var reactions = _repo.GetReactions(photo.Id);
        Assert.AreEqual(ReactionType.Like, reactions[_author.Id]);
    }

    [TestMethod]
    public void SetReaction_SameReactionAgain_TogglesOff()
    {
        var photo = NewPhoto();
        _repo.SetReaction(photo.Id, _author.Id, ReactionType.Like);

        _repo.SetReaction(photo.Id, _author.Id, ReactionType.Like);

        var reactions = _repo.GetReactions(photo.Id);
        Assert.IsFalse(reactions.ContainsKey(_author.Id));
    }

    [TestMethod]
    public void SetReaction_DifferentReaction_ReplacesExisting()
    {
        var photo = NewPhoto();
        _repo.SetReaction(photo.Id, _author.Id, ReactionType.Like);

        _repo.SetReaction(photo.Id, _author.Id, ReactionType.Love);

        var reactions = _repo.GetReactions(photo.Id);
        Assert.AreEqual(ReactionType.Love, reactions[_author.Id]);
    }

    [TestMethod]
    public void GetReactions_NoReactions_ReturnsEmptyDictionary()
    {
        var photo = NewPhoto();

        var reactions = _repo.GetReactions(photo.Id);

        Assert.AreEqual(0, reactions.Count);
    }

    [TestMethod]
    public void ToggleBookmark_NotBookmarked_AddsBookmark()
    {
        var photo = NewPhoto();

        _repo.ToggleBookmark(photo.Id, _author.Id);

        var bookmarks = _repo.GetBookmarks(photo.Id);
        Assert.IsTrue(bookmarks.Contains(_author.Id));
    }

    [TestMethod]
    public void ToggleBookmark_AlreadyBookmarked_RemovesBookmark()
    {
        var photo = NewPhoto();
        _repo.ToggleBookmark(photo.Id, _author.Id);

        _repo.ToggleBookmark(photo.Id, _author.Id);

        var bookmarks = _repo.GetBookmarks(photo.Id);
        Assert.IsFalse(bookmarks.Contains(_author.Id));
    }

    [TestMethod]
    public void GetBookmarks_NoBookmarks_ReturnsEmptySet()
    {
        var photo = NewPhoto();

        var bookmarks = _repo.GetBookmarks(photo.Id);

        Assert.AreEqual(0, bookmarks.Count);
    }
}