using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Posts;

namespace SocialNetworkingPlatform.Interfaces;

public interface IReelRepo
{
    Reel CreateReel(ReelDTO reel);
    bool RemoveReelById(Guid id);
    Reel UpdateReelById(Guid id, string newContent);
    Reel GetReelById(Guid id);
    List<Reel> GetAllReels();
}