using System;
using System.Collections.Generic;
using System.Text;

using SocialPlatformLibrary;

namespace SocialPlatformLibrary.DTO;

/// <summary>Input data required to create a new user.</summary>
public record UserDTO(string Name, string Email, string Password);

/// <summary>Input data required to create a new reel.</summary>
public record ReelDTO(User Author, string Content);

/// <summary>Input data required to create a new story.</summary>
public record StoryDTO(User Author, string Content);

/// <summary>Input data required to create a new photo post.</summary>
public record PhotoDTO(User Author, string Content, string PhotoUrl);

/// <summary>Input data required to create a new comment.
/// ParentId links to the post being commented on.</summary>
public record CommentDTO(User Author, string Content, Guid ParentId);
