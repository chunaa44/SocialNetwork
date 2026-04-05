using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Interfaces;

public interface ILikable
{
    HashSet<Guid> Likes { get; }
    void ToggleLike(Guid userId);
}
