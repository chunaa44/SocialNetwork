using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;
using SocialPlatformLibrary.Services;
using System;
using System.Collections.Generic;

namespace SocialPlatformLibrary;

public class Platform
{
    // Services used by platform
    private readonly UserService _userService;
    private readonly StoryService _storyService;
    private readonly ReelService _reelService;
    private readonly PhotoService _photoService;
    private readonly CommentService _commentService;

    // Platform constructor with dependency injection
    public Platform(
        UserService userService,
        StoryService storyService,
        ReelService reelService,
        PhotoService photoService,
        CommentService commentService)
    {
        _userService = userService;
        _storyService = storyService;
        _reelService = reelService;
        _photoService = photoService;
        _commentService = commentService;
    }

    // User operations
    /// <summary>
    /// Creates new User 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Created User</returns>
    public User CreateUser(UserDTO dto) => _userService.CreateUser(dto);

    /// <summary>
    /// Returns All Users in a List
    /// </summary>
    /// <returns>All users of a Platform in a List</returns>
    public List<User> GetAllUsers() => _userService.GetAllUsers();

    /// <summary>
    /// Get a User by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>User</returns>
    public User GetUserById(Guid id) => _userService.GetUserById(id);

    /// <summary>
    /// Update User's name and email
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="email"></param>
    /// <returns>Updated User</returns>
    public User UpdateUserById(Guid id, string name, string email) =>
        _userService.UpdateUserById(id, name, email);

    /// <summary>
    /// Remove User from platform by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true if succesfully removes User. false if else</returns>
    public bool RemoveUserById(Guid id) => _userService.RemoveUserById(id);

    // Follow/unfollow convenience methods
    /// <summary>
    /// Updates Users' following and followers list so that follower follows followee
    /// </summary>
    /// <param name="followerId"></param>
    /// <param name="followeeId"></param>
    /// <returns>true if both list successfully updates. false if else</returns>
    public bool FollowUser(Guid followerId, Guid followeeId) =>
        _userService.FollowUser(followerId, followeeId);

    /// <summary>
    /// Updates Users' following and followers list so that follower unfollows follwee
    /// </summary>
    /// <param name="followerId"></param>
    /// <param name="followeeId"></param>
    /// <returns>true if both list successfully updates. false if else</returns>
    public bool UnfollowUser(Guid followerId, Guid followeeId) =>
        _userService.UnfollowUser(followerId, followeeId);

    /// <summary>
    /// Get followers of a User with given id  
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>followers in a List<User></returns>
    public List<User> GetFollowers(Guid userId) => _userService.GetFollowers(userId);

    /// <summary>
    /// Get following of a User with given id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>following in a List<User></returns>
    public List<User> GetFollowing(Guid userId) => _userService.GetFollowing(userId);

    // Story operations

    /// <summary>
    /// Creates story 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Created story</returns>
    public Story CreateStory(StoryDTO dto) => _storyService.CreateStory(dto);

    /// <summary>
    /// Returns all stories in a list
    /// </summary>
    /// <returns>All stories of a platform in a list</returns>
    public List<Story> GetAllStories() => _storyService.GetAllStories();

    /// <summary>
    /// Updates content of an active Story with given id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newContent"></param>
    /// <returns>Updated Story</returns>
    public Story UpdateStoryById(Guid id, string newContent) => 
        _storyService.UpdateStoryById(id, newContent);

    /// <summary>
    /// Retuens all active Stories of a platform in a list
    /// </summary>
    /// <returns>active stories of a platfrom</returns>
    public List<Story> GetActiveStories() => _storyService.GetActiveStories();

    /// <summary>
    /// Get story with given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Story</returns>
    public Story GetStoryById(Guid id) => _storyService.GetStoryById(id);

    /// <summary>
    /// Remove story with given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true if successfully removed the story. false if else</returns>
    public bool RemoveStoryById(Guid id) => _storyService.RemoveStoryById(id);

    /// <summary>
    /// Add a view for a unique user
    /// </summary>
    /// <param name="storyId"></param>
    /// <param name="userId"></param>
    /// <returns>true when a new view was registered. false if else</returns>
    public bool AddView(Guid storyId, Guid userId) => 
        _storyService.AddView(storyId, userId);

    /// <summary>
    /// Toggle like of likable post 
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="userId"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public void ToggleLike(Guid postId, Guid userId)
    {
        var story = _storyService.GetStoryById(postId);
        if (story != null)
        {
            _storyService.ToggleLikeStory(postId, userId);
            return;
        }

        var reel = _reelService.GetReelById(postId);
        if (reel != null)
        {
            _reelService.ToggleLikeReel(postId, userId);
            return;
        }

        var photo = _photoService.GetPhotoById(postId);
        if (photo != null)
        {
            _photoService.ToggleLikePhoto(postId, userId);
            return;
        }

        var comment = _commentService.GetCommentById(postId);
        if (comment != null)
        {
            _commentService.ToggleLikeComment(postId, userId);
            return;
        }

        throw new KeyNotFoundException("Entity not found");
    }

    /// <summary>
    /// Remove all expired stories from the platform
    /// </summary>
    /// <returns>count of how many stories are removed</returns>
    public int RemoveExpiredStories() => _storyService.RemoveExpiredStories();



    // Reel operations

    /// <summary>
    /// creates new reel
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>created reel</returns>
    public Reel CreateReel(ReelDTO dto) => _reelService.CreateReel(dto);

    /// <summary>
    /// Get all reels of the platform in a list
    /// </summary>
    /// <returns>list of reels</returns>
    public List<Reel> GetAllReels() => _reelService.GetAllReels();

    /// <summary>
    /// updates content of reel
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newContent"></param>
    /// <returns> updated reel</returns>
    public Reel UpdateReelById(Guid id, string newContent) =>
        _reelService.UpdateReelById(id, newContent);

    /// <summary>
    /// Get a reel by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>reel with given id</returns>
    public Reel GetReelById(Guid id) => _reelService.GetReelById(id);

    /// <summary>
    /// Removes reel from platform
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true if successfully removes reel. false if else</returns>
    public bool RemoveReelById(Guid id) => _reelService.RemoveReelById(id);

    // Photo operations

    /// <summary>
    /// Creates photo
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>created photo</returns>
    public Photo CreatePhoto(PhotoDTO dto) => _photoService.CreatePhoto(dto);

    /// <summary>
    /// Returns all photos of the platform in a list
    /// </summary>
    /// <returns>list of photos</returns>
    public List<Photo> GetAllPhotos() => _photoService.GetAllPhotos();

    /// <summary>
    /// Updates content and photo url of the photo object
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newContent"></param>
    /// <param name="newPhotoURL"></param>
    /// <returns>Updated photo</returns>
    public Photo UpdatePhotoById(Guid id, string newContent, string newPhotoURL) =>
        _photoService.UpdatePhotoById(id, newContent, newPhotoURL);

    /// <summary>
    /// Gets photo 
    /// </summary>
    /// <param name="id"></param>
    /// <returns>photo with given id</returns>
    public Photo GetPhotoById(Guid id) => _photoService.GetPhotoById(id);

    /// <summary>
    /// Removes photo
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true if successfully removed photo. false if else</returns>
    public bool RemovePhotoById(Guid id) => _photoService.RemovePhotoById(id);

    /// <summary>
    /// Toggle bookmark of bookmarkable post
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="userId"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public void ToggleBookmark(Guid postId, Guid userId)
    {
        var photo = _photoService.GetPhotoById(postId);
        if (photo != null)
        {
            _photoService.ToggleBookmarkPhoto(postId, userId);
            return;
        }

        throw new KeyNotFoundException("Entity not found");
    }

    /// <summary>
    /// creates comment and attach it to it's parent post
    /// </summary>
    /// <param name="comment"></param>
    /// <param name="target"></param>
    /// <returns>Created comment</returns>
    public Comment CreateComment(CommentDTO comment, ICommentable target) =>
        _commentService.CreateComment(comment, target);

    /// <summary>
    /// update content of a comment
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newContent"></param>
    /// <returns>updated comment</returns>
    public Comment UpdateCommentById(Guid id, string newContent) =>
        _commentService.UpdateCommentById(id, newContent);

    /// <summary>
    /// get all comments of the platform in a list
    /// </summary>
    /// <returns>List of comments</returns>
    public List<Comment> GetAllComments() =>
        _commentService.GetAllComments();

    /// <summary>
    /// Get comment by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Comment</returns>
    public Comment GetCommentById(Guid id) =>
        _commentService.GetCommentById(id);

    /// <summary>
    /// Remove comment and detatch it from the parent post
    /// </summary>
    /// <param name="id"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool RemoveCommentById(Guid id, ICommentable target) =>
        _commentService.RemoveCommentById(id, target);


}