using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;
using System.Linq;

namespace SocialPlatformLibrary.Repositories;

public class ReelRepoMemory : IReelRepo
{
    List<Reel> reels = new List<Reel>();

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

    public Reel GetReelById(Guid id)
    {
        return reels.FirstOrDefault(r => r.Id == id);
    }

    public bool RemoveReelById(Guid id)
    {
        int removed = reels.RemoveAll(r => r.Id == id);
        return removed > 0;
    }

    public Reel UpdateReelById(Guid id, string newContent)
    {
        var reel = GetReelById(id);

        if (reel == null)
            return null;

        reel.Content = newContent;
        return reel;
    }
}