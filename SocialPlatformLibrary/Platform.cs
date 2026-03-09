using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Posts;
using SocialNetworkingPlatform.Services;

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

    public User CreateUser(UserDTO dto)
    {
        return _userService.CreateUser(dto);
    }

    public List<User> GetAllUsers()
    {
        return _userService.GetAllUsers();
    }

    public Story CreateStory(StoryDTO dto)
    {
        return _storyService.CreateStory(dto);
    }

    public List<Story> GetAllStories()
    {
        return _storyService.GetAllStories();
    }

    public Reel CreateReel(ReelDTO dto)
    {
        return _reelService.CreateReel(dto);
    }

    public List<Reel> GetAllReels()
    {
        return _reelService.GetAllReels();
    }

    public Photo CreatePhoto(PhotoDTO dto)
    {
        return _photoService.CreatePhoto(dto);
    }

    public List<Photo> GetAllPhotos()
    {
        return _photoService.GetAllPhotos();
    }
}