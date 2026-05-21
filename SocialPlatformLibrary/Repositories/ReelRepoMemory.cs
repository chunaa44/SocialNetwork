using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;
using System.Linq;

namespace SocialPlatformLibrary.Repositories;

public class ReelRepoMemory : IReelRepo
{
    // simple in memory store
    List<Reel> reels = new List<Reel>();

    /// <summary>Creates and stores a new reel from the given DTO.</summary>
    public Reel CreateReel(ReelDTO reel)
    {
        var newReel = new Reel()
        {
            AuthorId = reel.Author.Id,
            Content = reel.Content
        };

        reels.Add(newReel);
        return newReel;
    }

    public List<Reel> GetAllReels()
    {
        return reels;
    }

    /// <summary>Returns the reel with the given ID, or null if not found.</summary>
    public Reel GetReelById(Guid id)
    {
        return reels.FirstOrDefault(r => r.Id == id);
    }

    /// <summary>Removes the reel with the given ID. 
    /// Returns true if removed, false if not found.</summary>
    public bool RemoveReelById(Guid id)
    {
        int removed = reels.RemoveAll(r => r.Id == id);
        return removed > 0;
    }

    /// <summary>Updates the content of an existing reel. 
    /// Returns null if not found.</summary>
    public Reel UpdateReelById(Guid id, string newContent)
    {
        var reel = GetReelById(id);

        if (reel == null)
            return null;

        reel.Content = newContent;
        return reel;
    }
}