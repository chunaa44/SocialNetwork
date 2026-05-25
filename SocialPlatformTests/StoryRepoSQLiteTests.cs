using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Repositories;
using System;

namespace SocialPlatformTests;

[TestClass]
public class StoryRepoSQLiteTests : SqliteTestBase
{
    private StoryRepoSQLite _repo = null!;
    private User _author = null!;

    [TestInitialize]
    public void Setup()
    {
        _repo = new StoryRepoSQLite(Connection);
        _author = new User { Name = "Carol", Email = "carol@example.com", Password = "password123" };
    }

    private StoryDTO MakeDto(string content = "My story") => new StoryDTO(_author, content);

    // ── CreateStory ──────────────────────────────────────────────────────────

    [TestMethod]
    public void CreateStory_ReturnsStoryWithCorrectFields()
    {
        var story = _repo.CreateStory(MakeDto());

        Assert.AreNotEqual(Guid.Empty, story.Id);
        Assert.AreEqual(_author.Id, story.AuthorId);
        Assert.AreEqual("My story", story.Content);
    }

    [TestMethod]
    public void CreateStory_ExpiresAt_IsApproximately24HoursFromNow()
    {
        var story = _repo.CreateStory(MakeDto());

        var expectedExpiry = DateTime.Now.AddHours(24);
        // Allow ±5 seconds tolerance
        Assert.IsTrue(Math.Abs((story.ExpiresAt - expectedExpiry).TotalSeconds) < 5);
    }

    [TestMethod]
    public void CreateStory_IsPersisted_CanBeRetrieved()
    {
        var created = _repo.CreateStory(MakeDto());
        var found = _repo.GetStoryById(created.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found.Id);
    }

    [TestMethod]
    public void CreateStory_TwoStories_HaveUniqueIds()
    {
        var s1 = _repo.CreateStory(MakeDto("A"));
        var s2 = _repo.CreateStory(MakeDto("B"));

        Assert.AreNotEqual(s1.Id, s2.Id);
    }

    // ── GetAllStories ────────────────────────────────────────────────────────

    [TestMethod]
    public void GetAllStories_EmptyTable_ReturnsEmptyList()
    {
        Assert.AreEqual(0, _repo.GetAllStories().Count);
    }

    [TestMethod]
    public void GetAllStories_AfterInsertingTwo_ReturnsBoth()
    {
        _repo.CreateStory(MakeDto("A"));
        _repo.CreateStory(MakeDto("B"));

        Assert.AreEqual(2, _repo.GetAllStories().Count);
    }

    // ── GetStoryById ─────────────────────────────────────────────────────────

    [TestMethod]
    public void GetStoryById_ExistingId_ReturnsCorrectStory()
    {
        var created = _repo.CreateStory(MakeDto());
        var found = _repo.GetStoryById(created.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found.Id);
        Assert.AreEqual(created.Content, found.Content);
        Assert.AreEqual(created.AuthorId, found.AuthorId);
    }

    [TestMethod]
    public void GetStoryById_ExpiresAt_IsRestoredCorrectlyFromDb()
    {
        var created = _repo.CreateStory(MakeDto());
        var found = _repo.GetStoryById(created.Id);

        // ExpiresAt should survive a round-trip through the DB
        Assert.AreEqual(
            created.ExpiresAt.ToString("O"),
            found!.ExpiresAt.ToString("O"));
    }

    [TestMethod]
    public void GetStoryById_UnknownId_ReturnsNull()
    {
        Assert.IsNull(_repo.GetStoryById(Guid.NewGuid()));
    }

    // ── UpdateStoryById ──────────────────────────────────────────────────────

    [TestMethod]
    public void UpdateStoryById_ExistingId_UpdatesContent()
    {
        var story = _repo.CreateStory(MakeDto());
        var updated = _repo.UpdateStoryById(story.Id, "Updated content");

        Assert.IsNotNull(updated);
        Assert.AreEqual("Updated content", updated.Content);
    }

    [TestMethod]
    public void UpdateStoryById_ExistingId_ChangesArePersistedInDb()
    {
        var story = _repo.CreateStory(MakeDto());
        _repo.UpdateStoryById(story.Id, "Persisted update");

        var fromDb = _repo.GetStoryById(story.Id);
        Assert.AreEqual("Persisted update", fromDb!.Content);
    }

    [TestMethod]
    public void UpdateStoryById_UnknownId_ReturnsNull()
    {
        Assert.IsNull(_repo.UpdateStoryById(Guid.NewGuid(), "X"));
    }

    // ── RemoveStoryById ──────────────────────────────────────────────────────

    [TestMethod]
    public void RemoveStoryById_ExistingId_ReturnsTrueAndDeletesRow()
    {
        var story = _repo.CreateStory(MakeDto());
        bool removed = _repo.RemoveStoryById(story.Id);

        Assert.IsTrue(removed);
        Assert.IsNull(_repo.GetStoryById(story.Id));
    }

    [TestMethod]
    public void RemoveStoryById_UnknownId_ReturnsFalse()
    {
        Assert.IsFalse(_repo.RemoveStoryById(Guid.NewGuid()));
    }

    [TestMethod]
    public void RemoveStoryById_AlsoDeletesAssociatedLikes()
    {
        var story = _repo.CreateStory(MakeDto());
        _repo.ToggleLike(story.Id, Guid.NewGuid());
        _repo.RemoveStoryById(story.Id);

        Assert.AreEqual(0, _repo.GetLikes(story.Id).Count);
    }

    // ── ToggleLike ───────────────────────────────────────────────────────────

    [TestMethod]
    public void ToggleLike_FirstCall_AddsLike()
    {
        var story = _repo.CreateStory(MakeDto());
        var userId = Guid.NewGuid();

        _repo.ToggleLike(story.Id, userId);

        Assert.IsTrue(_repo.GetLikes(story.Id).Contains(userId));
    }

    [TestMethod]
    public void ToggleLike_SecondCall_RemovesLike()
    {
        var story = _repo.CreateStory(MakeDto());
        var userId = Guid.NewGuid();

        _repo.ToggleLike(story.Id, userId);
        _repo.ToggleLike(story.Id, userId);

        Assert.IsFalse(_repo.GetLikes(story.Id).Contains(userId));
    }

    [TestMethod]
    public void ToggleLike_MultipleUsers_AllStoredSeparately()
    {
        var story = _repo.CreateStory(MakeDto());
        var u1 = Guid.NewGuid();
        var u2 = Guid.NewGuid();

        _repo.ToggleLike(story.Id, u1);
        _repo.ToggleLike(story.Id, u2);
        var likes = _repo.GetLikes(story.Id);

        Assert.AreEqual(2, likes.Count);
        Assert.IsTrue(likes.Contains(u1));
        Assert.IsTrue(likes.Contains(u2));
    }

    // ── GetLikes ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void GetLikes_NoLikes_ReturnsEmptySet()
    {
        var story = _repo.CreateStory(MakeDto());
        Assert.AreEqual(0, _repo.GetLikes(story.Id).Count);
    }

    [TestMethod]
    public void GetLikes_AfterLiking_ReturnsCorrectUserId()
    {
        var story = _repo.CreateStory(MakeDto());
        var userId = Guid.NewGuid();

        _repo.ToggleLike(story.Id, userId);
        var likes = _repo.GetLikes(story.Id);

        Assert.AreEqual(1, likes.Count);
        Assert.IsTrue(likes.Contains(userId));
    }
}