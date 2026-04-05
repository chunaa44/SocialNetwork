using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkingPlatform.Interfaces;

public interface ILikable
{
    HashSet<Guid> Likes { get; }
    void ToggleLike(Guid userId);
}
