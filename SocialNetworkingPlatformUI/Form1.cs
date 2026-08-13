using Microsoft.Data.Sqlite;
using SocialPlatformLibrary;
using SocialPlatformLibrary.Repositories;
using SocialPlatformLibrary.Services;

namespace SocialNetworkingPlatformUI;

/// <summary>
/// Main application window. Owns the SQLite connection and the Platform
/// facade, and swaps between sign-in, sign-up, and the feed panel — the
/// rest of the UI never touches SQLite directly, only Platform.
/// </summary>
public partial class Form1 : Form
{
    private readonly Platform _platform;

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

        // Platform is the single facade every panel below talks to.
        _platform = new Platform(userService, storyService, reelService, photoService, commentService);

        ShowSignIn(); // start on the sign-in screen
    }

    private void ShowSignIn()
    {
        Controls.Clear();
        var panel = new SignInPanel(_platform, OnAuthenticated, onSwitchToSignUp: ShowSignUp) { Dock = DockStyle.Fill };
        Controls.Add(panel);
    }

    private void ShowSignUp()
    {
        Controls.Clear();
        var panel = new SignUpPanel(_platform, OnAuthenticated, onSwitchToSignIn: ShowSignIn) { Dock = DockStyle.Fill };
        Controls.Add(panel);
    }

    // Called by either panel once Platform confirms a valid user (login
    // succeeded or signup created a new account) — moves on to the feed.
    private void OnAuthenticated(User user)
    {
        Controls.Clear();
        var feedPanel = new FeedPanel(_platform, user, onLogout: ShowSignIn) { Dock = DockStyle.Fill };
        Controls.Add(feedPanel);
    }
}