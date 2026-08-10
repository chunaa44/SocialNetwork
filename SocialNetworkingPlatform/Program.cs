using SocialPlatformLibrary;
using SocialPlatformLibrary.Posts;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Repositories;
using SocialPlatformLibrary.Services;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        using var connection = new SqliteConnection("Data Source=social.db");
        connection.Open();
        // enable casacding deletion so no orphan record would be created
        using (var pragmaCmd = connection.CreateCommand())
        {
            pragmaCmd.CommandText = "PRAGMA foreign_keys = ON; PRAGMA recursive_triggers = ON;";
            pragmaCmd.ExecuteNonQuery();
        }

        DbInitializer.Initialize(connection);

        // repos
        var userRepo = new UserRepoSQLite(connection);
        var photoRepo = new PhotoRepoSQLite(connection);
        var storyRepo = new StoryRepoSQLite(connection);
        var reelRepo = new ReelRepoSQLite(connection);
        var commentRepo = new CommentRepoSQLite(connection);

        // services
        var userService = new UserService(userRepo);
        var photoService = new PhotoService(photoRepo);
        var storyService = new StoryService(storyRepo);
        var reelService = new ReelService(reelRepo);
        var commentService = new CommentService(commentRepo);

        var instagram = new Platform(userService, storyService, reelService, photoService, commentService);

        // users
        var user1 = instagram.CreateUser(new UserDTO("user1", "user1@gmail.com", "user1pass"));
        var user2 = instagram.CreateUser(new UserDTO("user2", "user2@gmail.com", "user2pass"));
        var user3 = instagram.CreateUser(new UserDTO("user3", "user3@gmail.com", "user3pass"));

        Console.WriteLine($"User count: {instagram.GetAllUsers().Count}");

        // follow, unfollow
        instagram.FollowUser(user1.Id, user2.Id);
        instagram.FollowUser(user1.Id, user3.Id);
        instagram.FollowUser(user2.Id, user3.Id);

        Console.WriteLine("Followers count");
        Console.WriteLine($"{user1.Name}: {instagram.GetFollowers(user1.Id).Count}");
        Console.WriteLine($"{user2.Name}: {instagram.GetFollowers(user2.Id).Count}");
        Console.WriteLine($"{user3.Name}: {instagram.GetFollowers(user3.Id).Count}");

        instagram.UnfollowUser(user1.Id, user3.Id);
        Console.WriteLine("Followers count after user1 unfollows user3");
        Console.WriteLine($"{user1.Name}: {instagram.GetFollowers(user1.Id).Count}");
        Console.WriteLine($"{user2.Name}: {instagram.GetFollowers(user2.Id).Count}");
        Console.WriteLine($"{user3.Name}: {instagram.GetFollowers(user3.Id).Count}");

        // photo, bookmark
        var user1_photo1 = instagram.CreatePhoto(new PhotoDTO(user1, "user1 photo1 content", "instagram.com/user1_photo1"));

        instagram.ToggleBookmark(user1_photo1.Id, user3.Id);
        Console.WriteLine($"user1 photo1 bookmark count: {instagram.GetBookmarks(user1_photo1.Id).Count}");
        instagram.ToggleBookmark(user1_photo1.Id, user3.Id);
        Console.WriteLine($"user1 photo1 bookmark count after toggle bookmark: {instagram.GetBookmarks(user1_photo1.Id).Count}");

        // comment, reaction
        var user1_photo1_comment1 = instagram.CreateComment(new CommentDTO(user2, "nice photo", user1_photo1.Id), user1_photo1);
        instagram.SetReaction(user1_photo1_comment1.Id, user1.Id, ReactionType.Love);

        Console.WriteLine($"user1 photo1 comment1 reaction count: {instagram.GetReactions(user1_photo1_comment1.Id).Count}");

        // story, story view, remove story
        var user3_story1 = instagram.CreateStory(new StoryDTO(user3, "user3 story1 content"));
        instagram.AddView(user3_story1.Id, user1.Id);
        instagram.AddView(user3_story1.Id, user2.Id);

        Console.WriteLine($"story count: {instagram.GetAllStories().Count}");

        instagram.RemoveStoryById(user3_story1.Id);
        Console.WriteLine($"story count after remove story: {instagram.GetAllStories().Count}");

        // reel, reaction
        var user1_reel1 = instagram.CreateReel(new ReelDTO(user1, "user1 reel1 content"));
        instagram.SetReaction(user1_reel1.Id, user2.Id, ReactionType.Haha);
        Console.WriteLine($"user1 reel1 reaction count: {instagram.GetReactions(user1_reel1.Id).Count}");

        // fetch everything back from social.db to prove it persisted
        Console.WriteLine("\n--- Fetched from social.db ---");
        Console.WriteLine($"Photos:   {instagram.GetAllPhotos().Count}");
        Console.WriteLine($"Comments: {instagram.GetAllComments().Count}");
        Console.WriteLine($"Stories:  {instagram.GetAllStories().Count}");
        Console.WriteLine($"Reels:    {instagram.GetAllReels().Count}");
    }
}