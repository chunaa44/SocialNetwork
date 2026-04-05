using System;

namespace SocialPlatformLibrary.Interfaces
{
    public interface IViewTrackable
    {
        int ViewCount { get; }
        // Returns true if a new unique view was recorded
        bool AddView(Guid userId);
    }
}
