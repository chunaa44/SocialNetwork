using Microsoft.Data.Sqlite;
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

        var connection = new SqliteConnection("Data Source=social_ui.db");
        connection.Open();
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
        photoLabel.AutoSize = true;
        UpdatePhotoLabel();
        this.Controls.Add(photoLabel);

        // alice's reaction picker
        var aliceLabel = new Label { Text = "Alice:", Location = new Point(20, 100), AutoSize = true };
        var alicePicker = new ReactionPicker();
        alicePicker.Location = new Point(100, 95);
        alicePicker.Tag = alice;
        alicePicker.ReactionSelected += ReactionPicker_ReactionSelected;
        this.Controls.Add(aliceLabel);
        this.Controls.Add(alicePicker);

        // bob's reaction picker
        var bobLabel = new Label { Text = "Bob:", Location = new Point(20, 140), AutoSize = true };
        var bobPicker = new ReactionPicker();
        bobPicker.Location = new Point(100, 135);
        bobPicker.Tag = bob;
        bobPicker.ReactionSelected += ReactionPicker_ReactionSelected;
        this.Controls.Add(bobLabel);
        this.Controls.Add(bobPicker);
    }

    private void ReactionPicker_ReactionSelected(object? sender, ReactionType reaction)
    {
        var picker = (ReactionPicker)sender!;
        var user = (User)picker.Tag!;

        instagram.SetReaction(photo.Id, user.Id, reaction);

        var reactions = instagram.GetReactions(photo.Id);
        picker.CurrentReaction = reactions.TryGetValue(user.Id, out var r) ? r : null;

        UpdatePhotoLabel();
    }

    private void UpdatePhotoLabel()
    {
        photoLabel.Text = $"{photo.Content} - {instagram.GetReactions(photo.Id).Count} reactions";
    }
}