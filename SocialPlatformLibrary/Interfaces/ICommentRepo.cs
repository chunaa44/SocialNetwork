using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Posts;
using System;
using System.Collections.Generic;

namespace SocialPlatformLibrary.Interfaces;

public interface ICommentRepo
{
    public Comment CreateComment(CommentDTO comment);
    public bool RemoveCommentById(Guid id);
    public Comment UpdateCommentById(Guid id, string newContent);
    public Comment GetCommentById(Guid id);
    public List<Comment> GetAllComments();
}
