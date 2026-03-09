using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkingPlatform.DTO;

public record UserDTO(string name, string email, string password);

public record ReelDTO(User author, string content);

public record StoryDTO(User author, string content);

public record PhotoDTO(User author, string content, string photoUrl);
