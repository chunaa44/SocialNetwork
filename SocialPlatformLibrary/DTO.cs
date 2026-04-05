using System;
using System.Collections.Generic;
using System.Text;

using SocialPlatformLibrary;

namespace SocialPlatformLibrary.DTO;

public record UserDTO(string Name, string Email, string Password);

public record ReelDTO(User Author, string Content);

public record StoryDTO(User Author, string Content);

public record PhotoDTO(User Author, string Content, string PhotoUrl);
