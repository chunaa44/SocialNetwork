using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary;

/// <summary>
/// Represents a registered user on the platform.
/// </summary>
public class User
{
    // Unique identifier assigned at creation, never changes.
    // init (not set) so repositories can reconstruct a User from a DB row via an object initializer.
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }

    // Users who follow this user
    public HashSet<Guid> Followers { get; } = new();

    // Users this user follows
    public HashSet<Guid> Following { get; } = new();
}