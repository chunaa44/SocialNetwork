using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Posts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Interfaces;

public interface ICommentable
{
    List<Comment> Comments { get; }
    void AddComment(Comment comment);
    void RemoveCommentById(Guid commentId);
}
