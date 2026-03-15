using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Posts;
using SocialNetworkingPlatform.Services;
using System;
using System.Collections.Generic;

namespace SocialNetworkingPlatform;

public class Platform
{
    // Services used by platform
    private readonly UserService _userService;
    private readonly StoryService _storyService;
    private readonly ReelService _reelService;
    private readonly PhotoService _photoService;

    // Platform constructor with dependency injection
    public Platform(
        UserService userService,
        StoryService storyService,
        ReelService reelService,
        PhotoService photoService)
    {
        _userService = userService;
        _storyService = storyService;
        _reelService = reelService;
        _photoService = photoService;
    }

    // User operations
    public User CreateUser(UserDTO dto) => _userService.CreateUser(dto);

    public List<User> GetAllUsers() => _userService.GetAllUsers();

    public User GetUserById(Guid id) => _userService.GetUserById(id);

    public User UpdateUserById(Guid id, string name, string email) =>
        _userService.UpdateUserById(id, name, email);

    public bool RemoveUserById(Guid id) => _userService.RemoveUserById(id);

    // Follow/unfollow convenience methods
    public bool FollowUser(Guid followerId, Guid followeeId) =>
        _userService.FollowUser(followerId, followeeId);

    public bool UnfollowUser(Guid followerId, Guid followeeId) =>
        _userService.UnfollowUser(followerId, followeeId);

    public List<User> GetFollowers(Guid userId) => _userService.GetFollowers(userId);

    public List<User> GetFollowing(Guid userId) => _userService.GetFollowing(userId);

    // Story operations
    public Story CreateStory(StoryDTO dto) => _storyService.CreateStory(dto);

    public List<Story> GetAllStories() => _storyService.GetAllStories();

    // Reel operations
    public Reel CreateReel(ReelDTO dto) => _reelService.CreateReel(dto);

    public List<Reel> GetAllReels() => _reelService.GetAllReels();

    // Photo operations
    public Photo CreatePhoto(PhotoDTO dto) => _photoService.CreatePhoto(dto);

    public List<Photo> GetAllPhotos() => _photoService.GetAllPhotos();
}