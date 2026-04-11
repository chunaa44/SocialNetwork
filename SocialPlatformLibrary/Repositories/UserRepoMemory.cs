using System;
using System.Collections.Generic;
using System.Linq;
using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;

namespace SocialPlatformLibrary.Repositories;

public class UserRepoMemory : IUserRepo
{
    List<User> users = new List<User>();

    public User CreateUser(UserDTO user)
    {
        var newUser = new User()
        {
            Name = user.Name,
            Email = user.Email,
            Password = user.Password
        };

        users.Add(newUser);
        return newUser;
    }

    public List<User> GetAllUsers()
    {
        return users;
    }

    public User GetUserById(Guid id)
    {
        return users.FirstOrDefault(u => u.Id == id);
    }

    public bool RemoveUserById(Guid id)
    {
        int removed = users.RemoveAll(u => u.Id == id);
        return removed > 0;
    }

    public User UpdateUserById(Guid id, string name, string email)
    {
        var user = GetUserById(id);

        if (user == null)
            return null;

        user.Name = name;
        user.Email = email;

        return user;
    }

    public bool FollowUser(Guid followerId, Guid followeeId)
    {
        if (followerId == Guid.Empty || followeeId == Guid.Empty)
            return false;

        if (followerId == followeeId)
            return false;

        var follower = GetUserById(followerId);
        var followee = GetUserById(followeeId);

        if (follower == null || followee == null)
            return false;

        bool addedToFollowing = follower.Following.Add(followeeId);
        bool addedToFollowers = followee.Followers.Add(followerId);

        return addedToFollowing && addedToFollowers;
    }

    public bool UnfollowUser(Guid followerId, Guid followeeId)
    {
        if (followerId == Guid.Empty || followeeId == Guid.Empty)
            return false;

        if (followerId == followeeId)
            return false;

        var follower = GetUserById(followerId);
        var followee = GetUserById(followeeId);

        if (follower == null || followee == null)
            return false;

        bool removedFromFollowing = follower.Following.Remove(followeeId);
        bool removedFromFollowers = followee.Followers.Remove(followerId);

        return removedFromFollowing && removedFromFollowers;
    }

    public List<User> GetFollowers(Guid userId)
    {
        var user = GetUserById(userId);
        if (user == null)
            return new List<User>();

        return users.Where(u => user.Followers.Contains(u.Id)).ToList();
    }

    public List<User> GetFollowing(Guid userId)
    {
        var user = GetUserById(userId);
        if (user == null)
            return new List<User>();

        return users.Where(u => user.Following.Contains(u.Id)).ToList();
    }
}