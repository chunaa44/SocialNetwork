using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Interfaces;

public interface IBookmarkable
{
    HashSet<Guid> Bookmarks { get; }
    void ToggleBookmark(Guid userId);
}
