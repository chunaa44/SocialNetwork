using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkingPlatform.Interfaces;

public interface ILikable
{
    int LikeCount { get; }
    void Like();
}
