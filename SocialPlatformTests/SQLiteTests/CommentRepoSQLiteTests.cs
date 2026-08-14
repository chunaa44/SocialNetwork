using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Posts;
using SocialPlatformLibrary.Repositories;
using SocialPlatformTests.SQLiteTests;

namespace SocialPlatformTests;

[TestClass]
public class CommentRepoSQLiteTests : SqliteTestBase
{
    private CommentRepoSQLite _repo = null!;
    private User _author = null!;
    private Guid _parentId;

    [TestInitialize]
    public void Init()
    {
        _repo = new CommentRepoSQLite(Connection);
        var userRepo = new UserRepoSQLite(Connection);
        _author = userRepo.CreateUser(new UserDTO("Alice", "alice@example.com", "password123"));
        _parentId = Guid.NewGuid();
    }

    private Comment NewComment(string content = "nice post")
        => _repo.CreateComment(new CommentDTO(_author, content, _parentId));

    [TestMethod]
    public void CreateComment_ReturnsCommentWithGeneratedId()
    {
        var comment = NewComment();

        Assert.AreNotEqual(Guid.Empty, comment.Id);
        Assert.AreEqual(_author.Id, comment.AuthorId);
        Assert.AreEqual(_parentId, comment.ParentId);
        Assert.AreEqual("nice post", comment.Content);
    }

    [TestMethod]
    public void GetCommentById_ExistingComment_ReturnsComment()
    {
        var created = NewComment();

        var found = _repo.GetCommentById(created.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found!.Id);
        Assert.AreEqual(created.ParentId, found.ParentId);
    }

    [TestMethod]
    public void GetCommentById_NonExistentComment_ReturnsNull()
    {
        var found = _repo.GetCommentById(Guid.NewGuid());

        Assert.IsNull(found);
    }

    [TestMethod]
    public void GetAllComments_ReturnsAllCreatedComments()
    {
        NewComment("first");
        NewComment("second");

        var all = _repo.GetAllComments();

        Assert.AreEqual(2, all.Count);
    }

    [TestMethod]
    public void UpdateCommentById_ExistingComment_UpdatesContent()
    {
        var created = NewComment();

        var updated = _repo.UpdateCommentById(created.Id, "updated content");

        Assert.IsNotNull(updated);
        Assert.AreEqual("updated content", updated!.Content);
    }

    [TestMethod]
    public void UpdateCommentById_NonExistentComment_ReturnsNull()
    {
        var updated = _repo.UpdateCommentById(Guid.NewGuid(), "content");

        Assert.IsNull(updated);
    }

    [TestMethod]
    public void RemoveCommentById_ExistingComment_RemovesAndReturnsTrue()
    {
        var created = NewComment();

        var removed = _repo.RemoveCommentById(created.Id);

        Assert.IsTrue(removed);
        Assert.IsNull(_repo.GetCommentById(created.Id));
    }

    [TestMethod]
    public void RemoveCommentById_NonExistentComment_ReturnsFalse()
    {
        var removed = _repo.RemoveCommentById(Guid.NewGuid());

        Assert.IsFalse(removed);
    }

    [TestMethod]
    public void SetReaction_NewReaction_InsertsReaction()
    {
        var comment = NewComment();

        _repo.SetReaction(comment.Id, _author.Id, ReactionType.Wow);

        var reactions = _repo.GetReactions(comment.Id);
        Assert.AreEqual(ReactionType.Wow, reactions[_author.Id]);
    }

    [TestMethod]
    public void SetReaction_SameReactionAgain_TogglesOff()
    {
        var comment = NewComment();
        _repo.SetReaction(comment.Id, _author.Id, ReactionType.Wow);

        _repo.SetReaction(comment.Id, _author.Id, ReactionType.Wow);

        var reactions = _repo.GetReactions(comment.Id);
        Assert.IsFalse(reactions.ContainsKey(_author.Id));
    }

    [TestMethod]
    public void SetReaction_DifferentReaction_ReplacesExisting()
    {
        var comment = NewComment();
        _repo.SetReaction(comment.Id, _author.Id, ReactionType.Wow);

        _repo.SetReaction(comment.Id, _author.Id, ReactionType.Like);

        var reactions = _repo.GetReactions(comment.Id);
        Assert.AreEqual(ReactionType.Like, reactions[_author.Id]);
    }

    [TestMethod]
    public void GetReactions_NoReactions_ReturnsEmptyDictionary()
    {
        var comment = NewComment();

        var reactions = _repo.GetReactions(comment.Id);

        Assert.AreEqual(0, reactions.Count);
    }
}