using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkingPlatform.Interfaces;

public interface ICommentable
{
    List<string> Comments { get; }
    void AddComment(string userName, string text);
}
