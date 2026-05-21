using System;
using System.Collections.Generic;
using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;

namespace SocialPlatformLibrary.Interfaces;

public interface IUserRepo
{
    /// <summary>Creates a new user and returns the created instance.</summary>
    User CreateUser(UserDTO user);

    /// <summary>Removes the user with the given ID. Returns true if removed.</summary>
    bool RemoveUserById(Guid id);

    /// <summary>Updates name and email of an existing user.
    /// Returns null if not found.</summary>
    User UpdateUserById(Guid id, string name, string email);

    /// <summary>Returns the user with the given ID, or null if not found.</summary>
    User GetUserById(Guid id);

    /// <summary>Returns all users in the store.</summary>
    List<User> GetAllUsers();

    /// <summary>Adds followeeId to follower's Following and 
    /// followerId to followee's Followers.</summary>
    bool FollowUser(Guid followerId, Guid followeeId);

    /// <summary>Removes the follow relationship between follower and followee.</summary>
    bool UnfollowUser(Guid followerId, Guid followeeId);

    /// <summary>Returns all users who follow the given user.</summary>
    List<User> GetFollowers(Guid userId);

    /// <summary>Returns all users the given user is following.</summary>
    List<User> GetFollowing(Guid userId);
}