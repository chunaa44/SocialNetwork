using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Repositories;
using System;

namespace SocialPlatformTests;

[TestClass]
public class CommentRepoSQLiteTests : SqliteTestBase
{
    private CommentRepoSQLite _repo = null!;
    private User _author = null!;
    private Guid _parentId;

    [TestInitialize]
    public void Setup()
    {
        _repo = new CommentRepoSQLite(Connection);
        _author = new User { Name = "Dave", Email = "dave@example.com", Password = "password123" };
        _parentId = Guid.NewGuid(); // fake parent post id
    }

    private CommentDTO MakeDto(string content = "Great post!") =>
        new CommentDTO(_author, content, _parentId);

    // ── CreateComment ────────────────────────────────────────────────────────

    [TestMethod]
    public void CreateComment_ReturnsCommentWithCorrectFields()
    {
        var comment = _repo.CreateComment(MakeDto());

        Assert.AreNotEqual(Guid.Empty, comment.Id);
        Assert.AreEqual(_author.Id, comment.AuthorId);
        Assert.AreEqual("Great post!", comment.Content);
        Assert.AreEqual(_parentId, comment.ParentId);
    }

    [TestMethod]
    public void CreateComment_IsPersisted_CanBeRetrieved()
    {
        var created = _repo.CreateComment(MakeDto());
        var found = _repo.GetCommentById(created.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found.Id);
    }

    [TestMethod]
    public void CreateComment_TwoComments_HaveUniqueIds()
    {
        var c1 = _repo.CreateComment(MakeDto("First"));
        var c2 = _repo.CreateComment(MakeDto("Second"));

        Assert.AreNotEqual(c1.Id, c2.Id);
    }

    [TestMethod]
    public void CreateComment_ParentId_IsPersistedCorrectly()
    {
        var created = _repo.CreateComment(MakeDto());
        var found = _repo.GetCommentById(created.Id);

        Assert.AreEqual(_parentId, found!.ParentId);
    }

    // ── GetAllComments ───────────────────────────────────────────────────────

    [TestMethod]
    public void GetAllComments_EmptyTable_ReturnsEmptyList()
    {
        Assert.AreEqual(0, _repo.GetAllComments().Count);
    }

    [TestMethod]
    public void GetAllComments_AfterInsertingTwo_ReturnsBoth()
    {
        _repo.CreateComment(MakeDto("A"));
        _repo.CreateComment(MakeDto("B"));

        Assert.AreEqual(2, _repo.GetAllComments().Count);
    }

    // ── GetCommentById ───────────────────────────────────────────────────────

    [TestMethod]
    public void GetCommentById_ExistingId_ReturnsCorrectComment()
    {
        var created = _repo.CreateComment(MakeDto());
        var found = _repo.GetCommentById(created.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found.Id);
        Assert.AreEqual(created.Content, found.Content);
        Assert.AreEqual(created.AuthorId, found.AuthorId);
        Assert.AreEqual(created.ParentId, found.ParentId);
    }

    [TestMethod]
    public void GetCommentById_UnknownId_ReturnsNull()
    {
        Assert.IsNull(_repo.GetCommentById(Guid.NewGuid()));
    }

    // ── UpdateCommentById ────────────────────────────────────────────────────

    [TestMethod]
    public void UpdateCommentById_ExistingId_UpdatesContent()
    {
        var comment = _repo.CreateComment(MakeDto());
        var updated = _repo.UpdateCommentById(comment.Id, "Edited comment");

        Assert.IsNotNull(updated);
        Assert.AreEqual("Edited comment", updated.Content);
    }

    [TestMethod]
    public void UpdateCommentById_ExistingId_ChangesArePersistedInDb()
    {
        var comment = _repo.CreateComment(MakeDto());
        _repo.UpdateCommentById(comment.Id, "Persisted edit");

        var fromDb = _repo.GetCommentById(comment.Id);
        Assert.AreEqual("Persisted edit", fromDb!.Content);
    }

    [TestMethod]
    public void UpdateCommentById_UnknownId_ReturnsNull()
    {
        Assert.IsNull(_repo.UpdateCommentById(Guid.NewGuid(), "X"));
    }

    // ── RemoveCommentById ────────────────────────────────────────────────────

    [TestMethod]
    public void RemoveCommentById_ExistingId_ReturnsTrueAndDeletesRow()
    {
        var comment = _repo.CreateComment(MakeDto());
        bool removed = _repo.RemoveCommentById(comment.Id);

        Assert.IsTrue(removed);
        Assert.IsNull(_repo.GetCommentById(comment.Id));
    }

    [TestMethod]
    public void RemoveCommentById_UnknownId_ReturnsFalse()
    {
        Assert.IsFalse(_repo.RemoveCommentById(Guid.NewGuid()));
    }

    [TestMethod]
    public void RemoveCommentById_AlsoDeletesAssociatedLikes()
    {
        var comment = _repo.CreateComment(MakeDto());
        _repo.ToggleLike(comment.Id, Guid.NewGuid());
        _repo.RemoveCommentById(comment.Id);

        Assert.AreEqual(0, _repo.GetLikes(comment.Id).Count);
    }

    // ── ToggleLike ───────────────────────────────────────────────────────────

    [TestMethod]
    public void ToggleLike_FirstCall_AddsLike()
    {
        var comment = _repo.CreateComment(MakeDto());
        var userId = Guid.NewGuid();

        _repo.ToggleLike(comment.Id, userId);

        Assert.IsTrue(_repo.GetLikes(comment.Id).Contains(userId));
    }

    [TestMethod]
    public void ToggleLike_SecondCall_RemovesLike()
    {
        var comment = _repo.CreateComment(MakeDto());
        var userId = Guid.NewGuid();

        _repo.ToggleLike(comment.Id, userId);
        _repo.ToggleLike(comment.Id, userId);

        Assert.IsFalse(_repo.GetLikes(comment.Id).Contains(userId));
    }

    [TestMethod]
    public void ToggleLike_MultipleUsers_AllStoredSeparately()
    {
        var comment = _repo.CreateComment(MakeDto());
        var u1 = Guid.NewGuid();
        var u2 = Guid.NewGuid();

        _repo.ToggleLike(comment.Id, u1);
        _repo.ToggleLike(comment.Id, u2);
        var likes = _repo.GetLikes(comment.Id);

        Assert.AreEqual(2, likes.Count);
        Assert.IsTrue(likes.Contains(u1));
        Assert.IsTrue(likes.Contains(u2));
    }

    // ── GetLikes ─────────────────────────────────────────────────────────────

    [TestMethod]
    public void GetLikes_NoLikes_ReturnsEmptySet()
    {
        var comment = _repo.CreateComment(MakeDto());
        Assert.AreEqual(0, _repo.GetLikes(comment.Id).Count);
    }

    [TestMethod]
    public void GetLikes_AfterLiking_ReturnsCorrectUserId()
    {
        var comment = _repo.CreateComment(MakeDto());
        var userId = Guid.NewGuid();

        _repo.ToggleLike(comment.Id, userId);
        var likes = _repo.GetLikes(comment.Id);

        Assert.AreEqual(1, likes.Count);
        Assert.IsTrue(likes.Contains(userId));
    }
}