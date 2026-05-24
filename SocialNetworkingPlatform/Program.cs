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
        // ── IN-MEMORY ────────────────────────────────────────────────────────
        Console.WriteLine("=== IN-MEMORY ===\n"); 

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
        Console.WriteLine($"user1 photo1 bookmark count: {user1_photo1.Bookmarks.Count}");
        instagram.ToggleBookmark(user1_photo1.Id, user3.Id);
        Console.WriteLine($"user1 photo1 bookmark count after toggle bookmark: {user1_photo1.Bookmarks.Count}");

        // comment, like
        var user1_photo1_comment1 = instagram.CreateComment(new CommentDTO(user2, "nice photo", user1_photo1.Id), user1_photo1);
        instagram.ToggleLike(user1_photo1_comment1.Id, user1.Id);

        Console.WriteLine($"user1 photo1 comment1 like count: {instagram.GetCommentById(user1_photo1_comment1.Id).Likes.Count}");

        // story, story view, remove story
        var user3_story1 = instagram.CreateStory(new StoryDTO(user3, "user3 story1 content"));
        instagram.AddView(user3_story1.Id, user1.Id);
        instagram.AddView(user3_story1.Id, user2.Id);

        Console.WriteLine($"story count: {instagram.GetAllStories().Count}");

        Console.WriteLine($"user3 story1 view count: {user3_story1.ViewCount}");


        instagram.RemoveStoryById(user3_story1.Id);

        Console.WriteLine($"story count after remove story: {instagram.GetAllStories().Count}");

        // ── SQLITE ───────────────────────────────────────────────────────────
        Console.WriteLine("\n=== SQLITE ===\n");

        using var connection = new SqliteConnection("Data Source=social.db");
        connection.Open();

        DbInitializer.Initialize(connection);

        // sqlite repos
        var sqlitePhotoRepo = new PhotoRepoSQLite(connection);
        var sqliteReelRepo = new ReelRepoSQLite(connection);
        var sqliteStoryRepo = new StoryRepoSQLite(connection);
        var sqliteCommentRepo = new CommentRepoSQLite(connection);

        // User doesn't have a SQLite repo 
        var sqliteUserRepo = new UserRepoMemory();

        // services
        var sqliteUserService = new UserService(sqliteUserRepo);
        var sqlitePhotoService = new PhotoService(sqlitePhotoRepo);
        var sqliteStoryService = new StoryService(sqliteStoryRepo);
        var sqliteReelService = new ReelService(sqliteReelRepo);
        var sqliteCommentService = new CommentService(sqliteCommentRepo);

        var snapchat = new Platform(sqliteUserService, sqliteStoryService, 
            sqliteReelService, sqlitePhotoService, sqliteCommentService);

        var alice = snapchat.CreateUser(new UserDTO("Alice", "alice@gmail.com", "alicepass"));
        var bob = snapchat.CreateUser(new UserDTO("Bob", "bob@gmail.com", "bobpass1"));

        // Create a photo — this INSERT goes into social.db
        var alicePhoto = snapchat.CreatePhoto(new PhotoDTO(alice, "My first photo", "snapchat.com/alice1"));
        Console.WriteLine($"Created photo with id: {alicePhoto.Id}");

        // Like the photo — this INSERT goes into the Likes table in social.db
        snapchat.ToggleLike(alicePhoto.Id, bob.Id);
        var likes = snapchat.GetLikes(alicePhoto.Id);
        Console.WriteLine($"Photo like count after bob likes: {likes.Count}");

        // Toggle again to unlike — this DELETE removes from Likes table
        snapchat.ToggleLike(alicePhoto.Id, bob.Id);
        likes = snapchat.GetLikes(alicePhoto.Id);
        Console.WriteLine($"Photo like count after bob unlikes: {likes.Count}");

        // Create a comment on the photo
        var bobComment = snapchat.CreateComment(
            new CommentDTO(bob, "Nice shot!", alicePhoto.Id),
            alicePhoto);
        Console.WriteLine($"Created comment: \"{bobComment.Content}\"");

        // Like the comment
        snapchat.ToggleLike(bobComment.Id, alice.Id);
        var commentLikes = snapchat.GetLikes(bobComment.Id);
        Console.WriteLine($"Comment like count: {commentLikes.Count}");

        // Create a story
        var aliceStory = snapchat.CreateStory(new StoryDTO(alice, "Story content here"));

        // Create a reel
        var aliceReel = snapchat.CreateReel(new ReelDTO(alice, "My first reel"));
        snapchat.ToggleLike(aliceReel.Id, bob.Id);
        var reelLikes = snapchat.GetLikes(aliceReel.Id);
        Console.WriteLine($"Reel like count: {reelLikes.Count}");

        // Fetch everything back from DB to prove it persisted
        Console.WriteLine("\n--- Fetched from DB ---");
        var allPhotos = snapchat.GetAllPhotos();
        var allComments = snapchat.GetAllComments();
        var allStories = snapchat.GetAllStories();
        var allReels = snapchat.GetAllReels();
        Console.WriteLine($"Photos in DB:   {allPhotos.Count}");
        Console.WriteLine($"Comments in DB: {allComments.Count}");
        Console.WriteLine($"Stories in DB:  {allStories.Count}");
        Console.WriteLine($"Reels in DB:    {allReels.Count}");


    }
}