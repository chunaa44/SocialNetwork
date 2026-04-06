using SocialPlatformLibrary;
using SocialPlatformLibrary.Posts;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Repositories;
using SocialPlatformLibrary.Services;

class Program
{
    static void Main()
    {
        // repos
        var userRepo = new UserRepoMemory();
        var photoRepo = new PhotoRepoMemory();
        var storyRepo = new StoryRepoMemory();
        var reelRepo = new ReelRepoMemory();
        var commentRepo = new CommentRepoMemory();

        // services
        var userService = new UserService(userRepo);
        var photoService = new PhotoService(photoRepo);
        var storyService = new StoryService(storyRepo);
        var reelService = new ReelService(reelRepo);
        var commentService = new CommentService(commentRepo);

        
        var instagram = new Platform(userService, storyService, reelService, photoService, commentService);

        
        var alice = instagram.CreateUser(new UserDTO("Alice", "alice@example.com", "password123"));
        var bob   = instagram.CreateUser(new UserDTO("Bob",   "bob@example.com",   "password123"));

        
        instagram.FollowUser(alice.Id, bob.Id);
        instagram.FollowUser(bob.Id, alice.Id);
        

        Console.WriteLine("Followers / Following:");
        Console.WriteLine($" - {alice.Name} follows: {instagram.GetFollowing(alice.Id).Count}");
        Console.WriteLine($" - {bob.Name} followers: {instagram.GetFollowers(bob.Id).Count}");

        
        var photo = instagram.CreatePhoto(new PhotoDTO(alice, "Lovely sunset", "https://example.com/sunset.jpg"));
        photoService.ToggleLikePhoto(photo.Id, bob.Id);

        var comment = commentService.CreateComment(new CommentDTO(bob, "woow", photo.Id), photo);
        var authorComment = userService.GetUserById(comment.AuthorId);
        Console.WriteLine($"{authorComment.Name}: {comment.Content}");

        List<Photo> photos = photoService.GetAllPhotos();
        Console.WriteLine(photos.Count);

        photoService.RemovePhotoById(photo.Id);
        photos = photoService.GetAllPhotos();
        Console.WriteLine(photos.Count);

        //commentService.RemoveCommentById(comment.Id, photo);


        Console.WriteLine($"\nPhoto by {alice.Name}: \"{photo.Content}\"");
        Console.WriteLine($"Likes: {photo.Likes.Count}");
    }
}