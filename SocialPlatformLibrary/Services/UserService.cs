using System;
using System.Collections.Generic;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Services;

public class UserService
{
    IUserRepo _repo;

    public UserService(IUserRepo repo)
    {
        _repo = repo;
    }

    public User CreateUser(UserDTO user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        ValidateName(user.Name);
        ValidateEmail(user.Email);
        ValidatePassword(user.Password);

        return _repo.CreateUser(user);
    }

    public User UpdateUserById(Guid id, string name, string email)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));

        ValidateName(name);
        ValidateEmail(email);

        return _repo.UpdateUserById(id, name, email);
    }

    public List<User> GetAllUsers() => _repo.GetAllUsers();

    public User GetUserById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));

        return _repo.GetUserById(id);
    }

    public bool RemoveUserById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id must be provided.", nameof(id));

        return _repo.RemoveUserById(id);
    }

    // Follow feature service methods

    public bool FollowUser(Guid followerId, Guid followeeId)
    {
        if (followerId == Guid.Empty)
            throw new ArgumentException("Follower id must be provided.", nameof(followerId));
        if (followeeId == Guid.Empty)
            throw new ArgumentException("Followee id must be provided.", nameof(followeeId));
        if (followerId == followeeId)
            throw new ArgumentException("User cannot follow themselves.");

        var follower = _repo.GetUserById(followerId) ?? throw new ArgumentException("Follower not found.", nameof(followerId));
        var followee = _repo.GetUserById(followeeId) ?? throw new ArgumentException("Followee not found.", nameof(followeeId));

        return _repo.FollowUser(followerId, followeeId);
    }

    public bool UnfollowUser(Guid followerId, Guid followeeId)
    {
        if (followerId == Guid.Empty)
            throw new ArgumentException("Follower id must be provided.", nameof(followerId));
        if (followeeId == Guid.Empty)
            throw new ArgumentException("Followee id must be provided.", nameof(followeeId));
        if (followerId == followeeId)
            throw new ArgumentException("User cannot unfollow themselves.");

        var follower = _repo.GetUserById(followerId) ?? throw new ArgumentException("Follower not found.", nameof(followerId));
        var followee = _repo.GetUserById(followeeId) ?? throw new ArgumentException("Followee not found.", nameof(followeeId));

        return _repo.UnfollowUser(followerId, followeeId);
    }

    public List<User> GetFollowers(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id must be provided.", nameof(userId));

        var user = _repo.GetUserById(userId) ?? throw new ArgumentException("User not found.", nameof(userId));

        return _repo.GetFollowers(userId);
    }

    public List<User> GetFollowing(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id must be provided.", nameof(userId));

        var user = _repo.GetUserById(userId) ?? throw new ArgumentException("User not found.", nameof(userId));

        return _repo.GetFollowing(userId);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required and cannot be empty.", nameof(name));

        if (name.Length > 30)
            throw new ArgumentException("Name must be 30 characters or less.", nameof(name));
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required and cannot be empty.", nameof(email));

        if (email.Length > 100)
            throw new ArgumentException("Email must be 100 characters or less.", nameof(email));
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required and cannot be empty.", nameof(password));

        if (password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters long.", nameof(password));

        if (password.Length > 100)
            throw new ArgumentException("Password must be less than 100 characters.", nameof(password));
    }
}