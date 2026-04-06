using System;

namespace SocialPlatformLibrary.Interfaces
{
    public interface IViewTrackable
    {
        HashSet<Guid> Viewers { get; }
        // Returns true if a new unique view was recorded
        bool AddView(Guid userId);
    }
}
