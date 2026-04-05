using System;
using System.Collections.Generic;
using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;

namespace SocialPlatformLibrary.Interfaces;

public interface IUserRepo
{
    User CreateUser(UserDTO user);
    bool RemoveUserById(Guid id);
    User UpdateUserById(Guid id, string name, string email);
    User GetUserById(Guid id);
    List<User> GetAllUsers();
    bool FollowUser(Guid followerId, Guid followeeId);
    bool UnfollowUser(Guid followerId, Guid followeeId);
    List<User> GetFollowers(Guid userId);
    List<User> GetFollowing(Guid userId);
}