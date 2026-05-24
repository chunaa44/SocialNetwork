using ReactionControl;
using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Posts;
using SocialPlatformLibrary.Repositories;
using SocialPlatformLibrary.Services;

namespace SocialNetworkingPlatformUI;

public partial class Form1 : Form
{
    private readonly Platform instagram;
    private readonly Photo photo;
    private readonly User bob;
    private readonly User alice;
    private readonly Label photoLabel;

    public Form1()
    {
        InitializeComponent();

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

        // platform
        instagram = new Platform(userService, storyService, reelService, photoService, commentService);

        // users
        alice = instagram.CreateUser(new UserDTO("Alice", "alice@example.com", "password123"));
        bob = instagram.CreateUser(new UserDTO("Bob", "bob@example.com", "password123"));

        // photo
        photo = instagram.CreatePhoto(new PhotoDTO(alice, "Lovely sunset", "https://example.com/sunset.jpg"));

        // label
        photoLabel = new Label();
        photoLabel.Location = new Point(100, 50);
        photoLabel.Size = new Size(300, 20);
        photoLabel.Text = $"{photo.Content}";
        this.Controls.Add(photoLabel);

        // alice like button
        LikeControl aliceLike = new LikeControl();
        aliceLike.Location = new Point(100, 100);
        aliceLike.Tag = alice;
        aliceLike.LikeChanged += Like_LikeChanged;
        this.Controls.Add(aliceLike);

        // bob like button
        LikeControl bobLike = new LikeControl();
        bobLike.Location = new Point(160, 100);
        bobLike.Tag = bob;
        bobLike.LikeChanged += Like_LikeChanged;
        this.Controls.Add(bobLike);


    }

    private void Like_LikeChanged(object sender, EventArgs e)
    {
        LikeControl lc = (LikeControl)sender;
        User user = (User)lc.Tag;

        instagram.ToggleLike(photo.Id, user.Id);

        photoLabel.Text = $"{photo.Content} - {photo.Likes.Count} likes";
    }
}
