using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Interfaces;

public interface IBookmarkable
{
    List<Guid> Bookmarks { get; }
    void Bookmark(Guid userId);
}
