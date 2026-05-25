using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Repositories;
using System;

namespace SocialPlatformTests;

[TestClass]
public class ReelRepoSQLiteTests : SqliteTestBase
{
    private ReelRepoSQLite _repo = null!;
    private User _author = null!;

    [TestInitialize]
    public void Setup()
    {
        _repo = new ReelRepoSQLite(Connection);
        _author = new User { Name = "Bob", Email = "bob@example.com", Password = "password123" };
    }

    private ReelDTO MakeDto(string content = "Cool reel") => new ReelDTO(_author, content);

    // ── CreateReel ───────────────────────────────────────────────────────────

    [TestMethod]
    public void CreateReel_ReturnsReelWithCorrectFields()
    {
        var reel = _repo.CreateReel(MakeDto());

        Assert.AreNotEqual(Guid.Empty, reel.Id);
        Assert.AreEqual(_author.Id, reel.AuthorId);
        Assert.AreEqual("Cool reel", reel.Content);
    }

    [TestMethod]
    public void CreateReel_IsPersisted_CanBeRetrieved()
    {
        var created = _repo.CreateReel(MakeDto());
        var found = _repo.GetReelById(created.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found.Id);
    }

    [TestMethod]
    public void CreateReel_TwoReels_HaveUniqueIds()
    {
        var r1 = _repo.CreateReel(MakeDto("First"));
        var r2 = _repo.CreateReel(MakeDto("Second"));

        Assert.AreNotEqual(r1.Id, r2.Id);
    }

    // ── GetAllReels ──────────────────────────────────────────────────────────

    [TestMethod]
    public void GetAllReels_EmptyTable_ReturnsEmptyList()
    {
        Assert.AreEqual(0, _repo.GetAllReels().Count);
    }

    [TestMethod]
    public void GetAllReels_AfterInsertingTwo_ReturnsBoth()
    {
        _repo.CreateReel(MakeDto("A"));
        _repo.CreateReel(MakeDto("B"));

        Assert.AreEqual(2, _repo.GetAllReels().Count);
    }

    // ── GetReelById ──────────────────────────────────────────────────────────

    [TestMethod]
    public void GetReelById_ExistingId_ReturnsCorrectReel()
    {
        var created = _repo.CreateReel(MakeDto());
        var found = _repo.GetReelById(created.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found.Id);
        Assert.AreEqual(created.Content, found.Content);
        Assert.AreEqual(created.AuthorId, found.AuthorId);
    }

    [TestMethod]
    public void GetReelById_UnknownId_ReturnsNull()
    {
        Assert.IsNull(_repo.GetReelById(Guid.NewGuid()));
    }

    // ── UpdateReelById ───────────────────────────────────────────────────────

    [TestMethod]
    public void UpdateReelById_ExistingId_UpdatesContent()
    {
        var reel = _repo.CreateReel(MakeDto());
        var updated = _repo.UpdateReelById(reel.Id, "New content");

        Assert.IsNotNull(updated);
        Assert.AreEqual("New content", updated.Content);
    }

    [TestMethod]
    public void UpdateReelById_ExistingId_ChangesArePersistedInDb()
    {
        var reel = _repo.CreateReel(MakeDto());
        _repo.UpdateReelById(reel.Id, "Persisted content");

        var fromDb = _repo.GetReelById(reel.Id);
        Assert.AreEqual("Persisted content", fromDb!.Content);
    }

    [TestMethod]
    public void UpdateReelById_UnknownId_ReturnsNull()
    {
        Assert.IsNull(_repo.UpdateReelById(Guid.NewGuid(), "X"));
    }

    // ── RemoveReelById ───────────────────────────────────────────────────────

    [TestMethod]
    public void RemoveReelById_ExistingId_ReturnsTrueAndDeletesRow()
    {
        var reel = _repo.CreateReel(MakeDto());
        bool removed = _repo.RemoveReelById(reel.Id);

        Assert.IsTrue(removed);
        Assert.IsNull(_repo.GetReelById(reel.Id));
    }

    [TestMethod]
    public void RemoveReelById_UnknownId_ReturnsFalse()
    {
        Assert.IsFalse(_repo.RemoveReelById(Guid.NewGuid()));
    }

    [TestMethod]
    public void RemoveReelById_AlsoDeletesAssociatedLikes()
    {
        var reel = _repo.CreateReel(MakeDto());
        _repo.ToggleLike(reel.Id, Guid.NewGuid());
        _repo.RemoveReelById(reel.Id);

        Assert.AreEqual(0, _repo.GetLikes(reel.Id).Count);
    }

    // ── ToggleLike ───────────────────────────────────────────────────────────

    [TestMethod]
    public void ToggleLike_FirstCall_AddsLike()
    {
        var reel = _repo.CreateReel(MakeDto());
        var userId = Guid.NewGuid();

        _repo.ToggleLike(reel.Id, userId);

        Assert.IsTrue(_repo.GetLikes(reel.Id).Contains(userId));
    }

    [TestMethod]
    public void ToggleLike_SecondCall_RemovesLike()
    {
        var reel = _repo.CreateReel(MakeDto());
        var userId = Guid.NewGuid();

        _repo.ToggleLike(reel.Id, userId);
        _repo.ToggleLike(reel.Id, userId);

        Assert.IsFalse(_repo.GetLikes(reel.Id).Contains(userId));
    }

    [TestMethod]
    public void ToggleLike_MultipleUsers_AllStoredSeparately()
    {
        var reel = _repo.CreateReel(MakeDto());
        var u1 = Guid.NewGuid();
        var u2 = Guid.NewGuid();

        _repo.ToggleLike(reel.Id, u1);
        _repo.ToggleLike(reel.Id, u2);
        var likes = _repo.GetLikes(reel.Id);

        Assert.AreEqual(2, likes.Count);
        Assert.IsTrue(likes.Contains(u1));
        Assert.IsTrue(likes.Contains(u2));
    }

    // ── GetLikes ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void GetLikes_NoLikes_ReturnsEmptySet()
    {
        var reel = _repo.CreateReel(MakeDto());
        Assert.AreEqual(0, _repo.GetLikes(reel.Id).Count);
    }

    [TestMethod]
    public void GetLikes_AfterLiking_ReturnsCorrectUserId()
    {
        var reel = _repo.CreateReel(MakeDto());
        var userId = Guid.NewGuid();

        _repo.ToggleLike(reel.Id, userId);
        var likes = _repo.GetLikes(reel.Id);

        Assert.AreEqual(1, likes.Count);
        Assert.IsTrue(likes.Contains(userId));
    }
}