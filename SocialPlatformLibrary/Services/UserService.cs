using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Interfaces;

namespace SocialNetworkingPlatform.Services;

public class UserService
{
    IUserRepo _repo;

    public UserService(IUserRepo repo)
    {
        _repo = repo;
    }

    public User CreateUser(UserDTO user) => _repo.CreateUser(user);

    public User UpdateUserById(Guid id, string name, string email)
        => _repo.UpdateUserById(id, name, email);

    public List<User> GetAllUsers() => _repo.GetAllUsers();

    public User GetUserById(Guid id) => _repo.GetUserById(id);

    public bool RemoveUserById(Guid id) => _repo.RemoveUserById(id);
}