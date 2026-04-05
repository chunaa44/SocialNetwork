using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Posts;

namespace SocialPlatformLibrary.Interfaces;

public interface IReelRepo
{
    public Reel CreateReel(ReelDTO reel);
    public bool RemoveReelById(Guid id);
    public Reel UpdateReelById(Guid id, string newContent);
    public Reel GetReelById(Guid id);
    public List<Reel> GetAllReels();
}