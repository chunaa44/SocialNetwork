using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ImageProcessorLibrary;
using ReactionControl;
using SocialPlatformLibrary;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Posts;

namespace SocialNetworkingPlatformUI;

/// <summary>
/// Main feed: shows every photo currently in the database, lets the logged-in
/// user upload a real image (processed through <see cref="ImageProcessor"/>) as
/// a new post, and react to posts through the <see cref="ReactionPicker"/> control.
/// All reads/writes go through <see cref="Platform"/> — no SQL here.
/// </summary>
public class FeedPanel : Panel
{
    private readonly Platform _platform;
    private readonly User _currentUser;
    private readonly Action _onLogout;
    private readonly ImageProcessor _imageProcessor = new();

    private readonly FlowLayoutPanel _postsContainer;
    private readonly TextBox _txtCaption;

    // Uploaded photos are saved here (next to the executable) so PhotoUrl
    // points at a real file the app can redisplay after a restart.
    private static readonly string PhotoStorageDir =
        Path.Combine(AppContext.BaseDirectory, "PhotoUploads");

    public FeedPanel(Platform platform, User currentUser, Action onLogout)
    {
        _platform = platform;
        _currentUser = currentUser;
        _onLogout = onLogout;

        Directory.CreateDirectory(PhotoStorageDir);

        var topBar = new Panel { Dock = DockStyle.Top, Height = 52 };

        // Shows who's currently logged in (comes from the User passed into
        // this panel by Form1, not re-queried from the DB).
        var lblWelcome = new Label
        {
            Text = $"Signed in as {_currentUser.Name}",
            Location = new Point(12, 18),
            AutoSize = true
        };

        // Caption entered here is read directly by BtnNewPost_Click below.
        _txtCaption = new TextBox
        {
            PlaceholderText = "Write a caption...",
            Location = new Point(220, 14),
            Size = new Size(220, 26)
        };

        var buttonSize = new Size(100, 30);
        int buttonY = 11;

        var btnNewPost = new Button { Text = "New Post", Location = new Point(450, buttonY), Size = buttonSize };
        btnNewPost.Click += BtnNewPost_Click;

        var btnRefresh = new Button { Text = "Refresh", Location = new Point(560, buttonY), Size = buttonSize };
        btnRefresh.Click += (_, _) => LoadFeed();

        var btnLogout = new Button { Text = "Log Out", Location = new Point(670, buttonY), Size = buttonSize };
        btnLogout.Click += (_, _) => _onLogout();

        topBar.Controls.Add(lblWelcome);
        topBar.Controls.Add(_txtCaption);
        topBar.Controls.Add(btnNewPost);
        topBar.Controls.Add(btnRefresh);
        topBar.Controls.Add(btnLogout);

        _postsContainer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown, // one post per row, scrolls vertically
            WrapContents = false
        };

        Controls.Add(_postsContainer);
        Controls.Add(topBar); // added last so it docks on top of the scroll area

        LoadFeed();
    }

    /// <summary>Reloads every photo from the database and rebuilds the post cards.</summary>
    public void LoadFeed()
    {
        _postsContainer.SuspendLayout();
        _postsContainer.Controls.Clear();

        var photos = _platform.GetAllPhotos().OrderByDescending(p => p.Timestamp);
        foreach (var photo in photos)
            _postsContainer.Controls.Add(BuildPostCard(photo));

        _postsContainer.ResumeLayout();
    }

    private Panel BuildPostCard(Photo photo)
    {
        // Photo only stores AuthorId, so look the User up to show their name.
        var author = _platform.GetUserById(photo.AuthorId);

        var card = new Panel
        {
            Width = 520,
            Height = 620,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(8)
        };

        var lblAuthor = new Label
        {
            Text = author?.Name ?? "Unknown user",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Location = new Point(10, 8),
            AutoSize = true
        };

        var pic = new PictureBox
        {
            Location = new Point(10, 32),
            Size = new Size(496, 496),
            SizeMode = PictureBoxSizeMode.Zoom, // scale to fit without distorting aspect ratio
            BorderStyle = BorderStyle.FixedSingle,
            Image = TryLoadImage(photo.PhotoUrl)
        };

        var lblContent = new Label
        {
            Text = photo.Content,
            Location = new Point(10, 534),
            Size = new Size(496, 40),
            AutoEllipsis = true
        };

        // Reactions is a Guid -> ReactionType map (one reaction per user per post).
        var reactions = _platform.GetReactions(photo.Id);

        var lblReactionCount = new Label
        {
            Text = $"{reactions.Count} reaction(s)",
            Location = new Point(10, 578),
            AutoSize = true
        };

        // Pre-select whichever reaction the current user already left (if any).
        var picker = new ReactionPicker
        {
            Location = new Point(140, 574),
            CurrentReaction = reactions.TryGetValue(_currentUser.Id, out var mine) ? mine : null
        };
        picker.ReactionSelected += (_, reaction) =>
        {
            // Persist immediately, then re-read from Platform so the count
            // and this user's selection both reflect the actual DB state.
            _platform.SetReaction(photo.Id, _currentUser.Id, reaction);
            var updated = _platform.GetReactions(photo.Id);
            picker.CurrentReaction = updated.TryGetValue(_currentUser.Id, out var r) ? r : null;
            lblReactionCount.Text = $"{updated.Count} reaction(s)";
        };

        card.Controls.Add(lblAuthor);
        card.Controls.Add(pic);
        card.Controls.Add(lblContent);
        card.Controls.Add(lblReactionCount);
        card.Controls.Add(picker);

        return card;
    }

    // Loads the image into memory first so the file on disk isn't left locked.
    private static Image? TryLoadImage(string path)
    {
        try
        {
            using var stream = new MemoryStream(File.ReadAllBytes(path));
            return Image.FromStream(stream);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private void BtnNewPost_Click(object? sender, EventArgs e)
    {
        string caption = _txtCaption.Text.Trim();
        if (string.IsNullOrWhiteSpace(caption))
        {
            MessageBox.Show(this, "Write a caption first.", "Missing caption", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Let the user pick a real image file from their computer.
        using var openDialog = new OpenFileDialog
        {
            Title = "Choose a photo",
            Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
        };

        if (openDialog.ShowDialog(this) != DialogResult.OK)
            return;

        // Not scoped inside the try below: ProcessImage can return this very
        // instance unchanged (when it's already 512x512), so it must stay
        // alive for as long as `processed` might still be pointing at it.
        using var original = new Bitmap(openDialog.FileName);

        // Hand off to the Image Processing library: crops to a square
        // aspect ratio and enforces the 512x512 size, or throws if the
        // source image is too small to begin with.
        Bitmap processed;
        try
        {
            processed = _imageProcessor.ProcessImage(original);
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(this, ex.Message, "Can't use this image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Detach from `original` so disposing one doesn't dispose the other.
        if (ReferenceEquals(processed, original))
            processed = new Bitmap(original);

        // Save the processed image to disk so PhotoUrl points at a real,
        // persistent file (not something that disappears when the app closes).
        string fileName = $"{Guid.NewGuid()}.png";
        string savedPath = Path.Combine(PhotoStorageDir, fileName);
        using (processed)
            processed.Save(savedPath, ImageFormat.Png);

        // Create the post through Platform -> PhotoService -> PhotoRepoSQLite,
        // then refresh the feed so the new post shows up immediately.
        _platform.CreatePhoto(new PhotoDTO(_currentUser, caption, savedPath));
        _txtCaption.Clear();
        LoadFeed();
    }
}