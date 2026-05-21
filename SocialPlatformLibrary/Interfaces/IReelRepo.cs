using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Interfaces;

public interface IReelRepo
{
    /// <summary>Creates a new reel and returns the created instance.</summary>
    public Reel CreateReel(ReelDTO reel);

    /// <summary>Removes the reel with the given ID. Returns true if removed.</summary>
    public bool RemoveReelById(Guid id);

    /// <summary>Updates content of an existing reel. Returns null if not found.</summary>
    public Reel UpdateReelById(Guid id, string newContent);

    /// <summary>Returns the reel with the given ID, or null if not found.</summary>
    public Reel GetReelById(Guid id);

    /// <summary>Returns all reels in the store.</summary>
    public List<Reel> GetAllReels();
}