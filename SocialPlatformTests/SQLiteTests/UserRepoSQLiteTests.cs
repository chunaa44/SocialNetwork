using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Repositories;
using SocialPlatformTests.SQLiteTests;

namespace SocialPlatformTests;

[TestClass]
public class UserRepoSQLiteTests : SqliteTestBase
{
    private UserRepoSQLite _repo = null!;

    [TestInitialize]
    public void Init()
    {
        _repo = new UserRepoSQLite(Connection);
    }

    private User NewUser(string name = "Alice", string email = "alice@example.com", string password = "password123")
        => _repo.CreateUser(new UserDTO(name, email, password));

    [TestMethod]
    public void CreateUser_ReturnsUserWithGeneratedId()
    {
        var user = NewUser();

        Assert.AreNotEqual(Guid.Empty, user.Id);
        Assert.AreEqual("Alice", user.Name);
        Assert.AreEqual("alice@example.com", user.Email);
        Assert.AreEqual("password123", user.Password);
    }

    [TestMethod]
    public void GetUserById_ExistingUser_ReturnsUser()
    {
        var created = NewUser();

        var found = _repo.GetUserById(created.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found!.Id);
        Assert.AreEqual(created.Name, found.Name);
    }

    [TestMethod]
    public void GetUserById_NonExistentUser_ReturnsNull()
    {
        var found = _repo.GetUserById(Guid.NewGuid());

        Assert.IsNull(found);
    }

    [TestMethod]
    public void GetUserByEmail_CaseInsensitiveMatch_ReturnsUser()
    {
        var created = NewUser(email: "Bob@Example.com");

        var found = _repo.GetUserByEmail("bob@example.com");

        Assert.IsNotNull(found);
        Assert.AreEqual(created.Id, found!.Id);
    }

    [TestMethod]
    public void GetUserByEmail_NonExistentEmail_ReturnsNull()
    {
        var found = _repo.GetUserByEmail("nobody@example.com");

        Assert.IsNull(found);
    }

    [TestMethod]
    public void GetAllUsers_ReturnsAllCreatedUsers()
    {
        NewUser(name: "Alice", email: "alice@example.com");
        NewUser(name: "Bob", email: "bob@example.com");

        var all = _repo.GetAllUsers();

        Assert.AreEqual(2, all.Count);
    }

    [TestMethod]
    public void UpdateUserById_ExistingUser_UpdatesNameAndEmail()
    {
        var created = NewUser();

        var updated = _repo.UpdateUserById(created.Id, "Alice Updated", "alice.new@example.com");

        Assert.IsNotNull(updated);
        Assert.AreEqual("Alice Updated", updated!.Name);
        Assert.AreEqual("alice.new@example.com", updated.Email);
    }

    [TestMethod]
    public void UpdateUserById_NonExistentUser_ReturnsNull()
    {
        var updated = _repo.UpdateUserById(Guid.NewGuid(), "Nobody", "nobody@example.com");

        Assert.IsNull(updated);
    }

    [TestMethod]
    public void RemoveUserById_ExistingUser_RemovesAndReturnsTrue()
    {
        var created = NewUser();

        var removed = _repo.RemoveUserById(created.Id);

        Assert.IsTrue(removed);
        Assert.IsNull(_repo.GetUserById(created.Id));
    }

    [TestMethod]
    public void RemoveUserById_NonExistentUser_ReturnsFalse()
    {
        var removed = _repo.RemoveUserById(Guid.NewGuid());

        Assert.IsFalse(removed);
    }

    [TestMethod]
    public void FollowUser_NewRelationship_ReturnsTrue()
    {
        var follower = NewUser(name: "Follower", email: "follower@example.com");
        var followee = NewUser(name: "Followee", email: "followee@example.com");

        var result = _repo.FollowUser(follower.Id, followee.Id);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void FollowUser_AlreadyFollowing_ReturnsFalse()
    {
        var follower = NewUser(name: "Follower", email: "follower@example.com");
        var followee = NewUser(name: "Followee", email: "followee@example.com");
        _repo.FollowUser(follower.Id, followee.Id);

        var result = _repo.FollowUser(follower.Id, followee.Id);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void UnfollowUser_ExistingRelationship_ReturnsTrue()
    {
        var follower = NewUser(name: "Follower", email: "follower@example.com");
        var followee = NewUser(name: "Followee", email: "followee@example.com");
        _repo.FollowUser(follower.Id, followee.Id);

        var result = _repo.UnfollowUser(follower.Id, followee.Id);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void UnfollowUser_NoExistingRelationship_ReturnsFalse()
    {
        var follower = NewUser(name: "Follower", email: "follower@example.com");
        var followee = NewUser(name: "Followee", email: "followee@example.com");

        var result = _repo.UnfollowUser(follower.Id, followee.Id);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetFollowers_ReturnsUsersFollowingTarget()
    {
        var target = NewUser(name: "Target", email: "target@example.com");
        var follower = NewUser(name: "Follower", email: "follower@example.com");
        _repo.FollowUser(follower.Id, target.Id);

        var followers = _repo.GetFollowers(target.Id);

        Assert.AreEqual(1, followers.Count);
        Assert.AreEqual(follower.Id, followers[0].Id);
    }

    [TestMethod]
    public void GetFollowers_NoFollowers_ReturnsEmptyList()
    {
        var target = NewUser();

        var followers = _repo.GetFollowers(target.Id);

        Assert.AreEqual(0, followers.Count);
    }

    [TestMethod]
    public void GetFollowing_ReturnsUsersTargetFollows()
    {
        var target = NewUser(name: "Target", email: "target@example.com");
        var followee = NewUser(name: "Followee", email: "followee@example.com");
        _repo.FollowUser(target.Id, followee.Id);

        var following = _repo.GetFollowing(target.Id);

        Assert.AreEqual(1, following.Count);
        Assert.AreEqual(followee.Id, following[0].Id);
    }

    [TestMethod]
    public void GetFollowing_NotFollowingAnyone_ReturnsEmptyList()
    {
        var target = NewUser();

        var following = _repo.GetFollowing(target.Id);

        Assert.AreEqual(0, following.Count);
    }
}