using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkingPlatform;

public class User
{
    public Guid Id { get; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public byte Age { get; init; }
}
