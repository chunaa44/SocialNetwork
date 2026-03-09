using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Interfaces;
using SocialNetworkingPlatform.Posts;

namespace SocialNetworkingPlatform.Services;

public class ReelService
{
    IReelRepo _repo;

    public ReelService(IReelRepo repo)
    {
        _repo = repo;
    }

    public Reel CreateReel(ReelDTO reel)
        => _repo.CreateReel(reel);

    public Reel UpdateReelById(Guid id, string newContent)
        => _repo.UpdateReelById(id, newContent);

    public List<Reel> GetAllReels()
        => _repo.GetAllReels();

    public Reel GetReelById(Guid id)
        => _repo.GetReelById(id);

    public bool RemoveReelById(Guid id)
        => _repo.RemoveReelById(id);
}