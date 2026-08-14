using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Posts;
using SocialPlatformLibrary.Repositories;
using SocialPlatformTests.SQLiteTests;

namespace SocialPlatformTests;

[TestClass]
public class ReelRepoSQLiteTests : SqliteTestBase
{
    private ReelRepoSQLite _repo = null!;
    private User _author = null!;

    [TestInitialize]
    public void Init()
    {
        _repo = new ReelRepoSQLite(Connection);
        var userRepo = new UserRepoSQLite(Connection);
        _author = userRepo.CreateUser(new UserDTO("Alice", "alice@example.com", "password123"));
    }

    private Reel NewReel(string content = "reel content")
        => _repo.CreateReel(new ReelDTO(_author, content));

    [TestMethod]
    public void CreateReel_ReturnsReelWithGeneratedId()
    {
        var reel = NewReel();

        Assert.AreNotEqual(Guid.Empty, reel.Id);
        Assert.AreEqual(_author.Id, reel.AuthorId);
        Assert.AreEqual("reel content", reel.Content);
    }

    [TestMethod]
    public void GetReelById_ExistingReel_ReturnsReel()
    {
        var created = NewReel();

        var found = _repo.GetReelById(created.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found!.Id);
    }

    [TestMethod]
    public void GetReelById_NonExistentReel_ReturnsNull()
    {
        var found = _repo.GetReelById(Guid.NewGuid());

        Assert.IsNull(found);
    }

    [TestMethod]
    public void GetAllReels_ReturnsAllCreatedReels()
    {
        NewReel("first");
        NewReel("second");

        var all = _repo.GetAllReels();

        Assert.AreEqual(2, all.Count);
    }

    [TestMethod]
    public void UpdateReelById_ExistingReel_UpdatesContent()
    {
        var created = NewReel();

        var updated = _repo.UpdateReelById(created.Id, "updated content");

        Assert.IsNotNull(updated);
        Assert.AreEqual("updated content", updated!.Content);
    }

    [TestMethod]
    public void UpdateReelById_NonExistentReel_ReturnsNull()
    {
        var updated = _repo.UpdateReelById(Guid.NewGuid(), "content");

        Assert.IsNull(updated);
    }

    [TestMethod]
    public void RemoveReelById_ExistingReel_RemovesAndReturnsTrue()
    {
        var created = NewReel();

        var removed = _repo.RemoveReelById(created.Id);

        Assert.IsTrue(removed);
        Assert.IsNull(_repo.GetReelById(created.Id));
    }

    [TestMethod]
    public void RemoveReelById_NonExistentReel_ReturnsFalse()
    {
        var removed = _repo.RemoveReelById(Guid.NewGuid());

        Assert.IsFalse(removed);
    }

    [TestMethod]
    public void SetReaction_NewReaction_InsertsReaction()
    {
        var reel = NewReel();

        _repo.SetReaction(reel.Id, _author.Id, ReactionType.Haha);

        var reactions = _repo.GetReactions(reel.Id);
        Assert.AreEqual(ReactionType.Haha, reactions[_author.Id]);
    }

    [TestMethod]
    public void SetReaction_SameReactionAgain_TogglesOff()
    {
        var reel = NewReel();
        _repo.SetReaction(reel.Id, _author.Id, ReactionType.Haha);

        _repo.SetReaction(reel.Id, _author.Id, ReactionType.Haha);

        var reactions = _repo.GetReactions(reel.Id);
        Assert.IsFalse(reactions.ContainsKey(_author.Id));
    }

    [TestMethod]
    public void SetReaction_DifferentReaction_ReplacesExisting()
    {
        var reel = NewReel();
        _repo.SetReaction(reel.Id, _author.Id, ReactionType.Haha);

        _repo.SetReaction(reel.Id, _author.Id, ReactionType.Wow);

        var reactions = _repo.GetReactions(reel.Id);
        Assert.AreEqual(ReactionType.Wow, reactions[_author.Id]);
    }

    [TestMethod]
    public void GetReactions_NoReactions_ReturnsEmptyDictionary()
    {
        var reel = NewReel();

        var reactions = _repo.GetReactions(reel.Id);

        Assert.AreEqual(0, reactions.Count);
    }
}