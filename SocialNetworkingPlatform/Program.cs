using SocialNetworkingPlatform;
using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Repositories;
using SocialNetworkingPlatform.Services;

class Program
{
    static void Main()
    {
        var userRepo = new UserRepoMemory();
        var photoRepo = new PhotoRepoMemory();

        var userService = new UserService(userRepo);
        var photoService = new PhotoService(photoRepo);

        var instagram = new Platform(userService, null, null, photoService);


        var user1 = instagram.CreateUser(new UserDTO(
            "Alice",
            "alice@mail.com",
            "123"
        ));

        var user2 = instagram.CreateUser(new UserDTO(
            "Bob",
            "bob@mail.com",
            "123"
        ));


        var photo = instagram.CreatePhoto(new PhotoDTO(
            user1,
            "Hello from Alice",
            "https://photo.com/a.jpg"
        ));

        photo.Like();

        Console.WriteLine($"User1: {user1.Name}");
        Console.WriteLine($"User2: {user2.Name}");

        Console.WriteLine($"\nPhoto Content: {photo.Content}");
        Console.WriteLine($"Likes on photo: {photo.LikeCount}");
    }
}