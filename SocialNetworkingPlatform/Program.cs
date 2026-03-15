using System;
using SocialNetworkingPlatform;
using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Repositories;
using SocialNetworkingPlatform.Services;

class Program
{
    static void Main()
    {
        // repos
        var userRepo = new UserRepoMemory();
        var photoRepo = new PhotoRepoMemory();
        var storyRepo = new StoryRepoMemory();
        var reelRepo = new ReelRepoMemory();

        // services
        var userService = new UserService(userRepo);
        var photoService = new PhotoService(photoRepo);
        var storyService = new StoryService(storyRepo);
        var reelService = new ReelService(reelRepo);

        
        var instagram = new Platform(userService, storyService, reelService, photoService);

        
        var alice = instagram.CreateUser(new UserDTO("Alice", "alice@example.com", "password123"));
        var bob   = instagram.CreateUser(new UserDTO("Bob",   "bob@example.com",   "password123"));

        
        instagram.FollowUser(alice.Id, bob.Id);
        instagram.FollowUser(bob.Id, alice.Id);

        Console.WriteLine("Followers / Following:");
        Console.WriteLine($" - {alice.Name} follows: {instagram.GetFollowing(alice.Id).Count}");
        Console.WriteLine($" - {bob.Name} followers: {instagram.GetFollowers(bob.Id).Count}");

        
        var photo = instagram.CreatePhoto(new PhotoDTO(alice, "Lovely sunset", "https://example.com/sunset.jpg"));
        photoService.LikePhoto(photo.Id); 

        Console.WriteLine($"\nPhoto by {alice.Name}: \"{photo.Content}\"");
        Console.WriteLine($"Likes: {photo.LikeCount}");
    }
}