using SocialNetworkingPlatform.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkingPlatform.Posts;

public class Story: Post, ILikable
{
    public int LikeCount { get; private set; }
    public void Like() => LikeCount++;
}
