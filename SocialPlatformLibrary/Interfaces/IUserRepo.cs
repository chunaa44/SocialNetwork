using SocialNetworkingPlatform.DTO;

namespace SocialNetworkingPlatform.Interfaces;

public interface IUserRepo
{
    User CreateUser(UserDTO user);
    bool RemoveUserById(Guid id);
    User UpdateUserById(Guid id, string name, string email);
    User GetUserById(Guid id);
    List<User> GetAllUsers();
}