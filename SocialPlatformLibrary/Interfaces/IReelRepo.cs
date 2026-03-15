using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Posts;

namespace SocialNetworkingPlatform.Interfaces;

public interface IReelRepo
{
    public Reel CreateReel(ReelDTO reel);
    public bool RemoveReelById(Guid id);
    public Reel UpdateReelById(Guid id, string newContent);
    public Reel GetReelById(Guid id);
    public List<Reel> GetAllReels();
}