using SocialNetworkingPlatform.DTO;
using SocialNetworkingPlatform.Interfaces;
using System.Linq;

namespace SocialNetworkingPlatform.Repositories;

public class UserRepoMemory : IUserRepo
{
    List<User> users = new List<User>();

    public User CreateUser(UserDTO user)
    {
        var newUser = new User()
        {
            Name = user.name,
            Email = user.email,
            Password = user.password
        };

        users.Add(newUser);
        return newUser;
    }

    public List<User> GetAllUsers()
    {
        return users;
    }

    public User GetUserById(Guid id)
    {
        return users.FirstOrDefault(u => u.Id == id);
    }

    public bool RemoveUserById(Guid id)
    {
        int removed = users.RemoveAll(u => u.Id == id);
        return removed > 0;
    }

    public User UpdateUserById(Guid id, string name, string email)
    {
        var user = GetUserById(id);

        if (user == null)
            return null;

        user.Name = name;
        user.Email = email;

        return user;
    }
}