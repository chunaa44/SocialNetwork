using System;
using System.Collections.Generic;
using System.Text;

using SocialPlatformLibrary;

namespace SocialPlatformLibrary.DTO;

/// <summary>Input data required to create a new user.</summary>
/// <param name="Name"></param>
/// <param name="Email"></param>
/// <param name="Password"></param>
public record UserDTO(string Name, string Email, string Password);

/// <summary>Input data required to create a new reel.</summary>
/// <param name="Author"></param>
/// <param name="Content"></param>
public record ReelDTO(User Author, string Content);

/// <summary>Input data required to create a new story.</summary>
/// <param name="Author"></param>
/// <param name="Content"></param>
public record StoryDTO(User Author, string Content);

/// <summary>Input data required to create a new photo post.</summary>
/// <param name="Author"></param>
/// <param name="Content"></param>
/// <param name="PhotoUrl"></param>
public record PhotoDTO(User Author, string Content, string PhotoUrl);

/// <summary>Input data required to create a new comment.
/// ParentId links to the post being commented on.</summary>
/// <param name="Author"></param>
/// <param name="Content"></param>
/// <param name="ParentId"></param>
public record CommentDTO(User Author, string Content, Guid ParentId);
