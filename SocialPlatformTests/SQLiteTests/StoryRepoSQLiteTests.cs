using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Posts;
using SocialPlatformLibrary.Repositories;
using SocialPlatformTests.SQLiteTests;

namespace SocialPlatformTests;

[TestClass]
public class StoryRepoSQLiteTests : SqliteTestBase
{
    private StoryRepoSQLite _repo = null!;
    private User _author = null!;
    private User _viewer = null!;

    [TestInitialize]
    public void Init()
    {
        _repo = new StoryRepoSQLite(Connection);
        var userRepo = new UserRepoSQLite(Connection);
        _author = userRepo.CreateUser(new UserDTO("Alice", "alice@example.com", "password123"));
        _viewer = userRepo.CreateUser(new UserDTO("Bob", "bob@example.com", "password123"));
    }

    private Story NewStory(string content = "story content")
        => _repo.CreateStory(new StoryDTO(_author, content));

    [TestMethod]
    public void CreateStory_ReturnsStoryWithGeneratedId()
    {
        var story = NewStory();

        Assert.AreNotEqual(Guid.Empty, story.Id);
        Assert.AreEqual(_author.Id, story.AuthorId);
        Assert.AreEqual("story content", story.Content);
    }

    [TestMethod]
    public void GetStoryById_ExistingStory_ReturnsStoryWithExpiresAt()
    {
        var created = NewStory();

        var found = _repo.GetStoryById(created.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found!.Id);
        Assert.AreEqual(created.ExpiresAt, found.ExpiresAt);
    }

    [TestMethod]
    public void GetStoryById_NonExistentStory_ReturnsNull()
    {
        var found = _repo.GetStoryById(Guid.NewGuid());

        Assert.IsNull(found);
    }

    [TestMethod]
    public void GetAllStories_ReturnsAllCreatedStories()
    {
        NewStory("first");
        NewStory("second");

        var all = _repo.GetAllStories();

        Assert.AreEqual(2, all.Count);
    }

    [TestMethod]
    public void UpdateStoryById_ExistingStory_UpdatesContent()
    {
        var created = NewStory();

        var updated = _repo.UpdateStoryById(created.Id, "updated content");

        Assert.IsNotNull(updated);
        Assert.AreEqual("updated content", updated!.Content);
    }

    [TestMethod]
    public void UpdateStoryById_NonExistentStory_ReturnsNull()
    {
        var updated = _repo.UpdateStoryById(Guid.NewGuid(), "content");

        Assert.IsNull(updated);
    }

    [TestMethod]
    public void RemoveStoryById_ExistingStory_RemovesAndReturnsTrue()
    {
        var created = NewStory();

        var removed = _repo.RemoveStoryById(created.Id);

        Assert.IsTrue(removed);
        Assert.IsNull(_repo.GetStoryById(created.Id));
    }

    [TestMethod]
    public void RemoveStoryById_NonExistentStory_ReturnsFalse()
    {
        var removed = _repo.RemoveStoryById(Guid.NewGuid());

        Assert.IsFalse(removed);
    }

    [TestMethod]
    public void SetReaction_NewReaction_InsertsReaction()
    {
        var story = NewStory();

        _repo.SetReaction(story.Id, _viewer.Id, ReactionType.Sad);

        var reactions = _repo.GetReactions(story.Id);
        Assert.AreEqual(ReactionType.Sad, reactions[_viewer.Id]);
    }

    [TestMethod]
    public void SetReaction_SameReactionAgain_TogglesOff()
    {
        var story = NewStory();
        _repo.SetReaction(story.Id, _viewer.Id, ReactionType.Sad);

        _repo.SetReaction(story.Id, _viewer.Id, ReactionType.Sad);

        var reactions = _repo.GetReactions(story.Id);
        Assert.IsFalse(reactions.ContainsKey(_viewer.Id));
    }

    [TestMethod]
    public void SetReaction_DifferentReaction_ReplacesExisting()
    {
        var story = NewStory();
        _repo.SetReaction(story.Id, _viewer.Id, ReactionType.Sad);

        _repo.SetReaction(story.Id, _viewer.Id, ReactionType.Angry);

        var reactions = _repo.GetReactions(story.Id);
        Assert.AreEqual(ReactionType.Angry, reactions[_viewer.Id]);
    }

    [TestMethod]
    public void GetReactions_NoReactions_ReturnsEmptyDictionary()
    {
        var story = NewStory();

        var reactions = _repo.GetReactions(story.Id);

        Assert.AreEqual(0, reactions.Count);
    }

    [TestMethod]
    public void AddView_NewViewer_ReturnsTrue()
    {
        var story = NewStory();

        var result = _repo.AddView(story.Id, _viewer.Id);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void AddView_AlreadyViewed_ReturnsFalse()
    {
        var story = NewStory();
        _repo.AddView(story.Id, _viewer.Id);

        var result = _repo.AddView(story.Id, _viewer.Id);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetViewers_ReturnsViewersWhoViewed()
    {
        var story = NewStory();
        _repo.AddView(story.Id, _viewer.Id);

        var viewers = _repo.GetViewers(story.Id);

        Assert.IsTrue(viewers.Contains(_viewer.Id));
    }

    [TestMethod]
    public void GetViewers_NoViewers_ReturnsEmptySet()
    {
        var story = NewStory();

        var viewers = _repo.GetViewers(story.Id);

        Assert.AreEqual(0, viewers.Count);
    }
}