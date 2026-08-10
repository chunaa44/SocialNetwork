using System;
using System.Collections.Generic;

namespace SocialPlatformLibrary.Interfaces;

/// <summary>
/// Marks a post as reactable. Any class implementing this can receive one of
/// several <see cref="ReactionType"/> reactions per user (like, love, haha, etc.).
/// </summary>
public interface IReactable
{
    /// <summary>Reactions on this post, keyed by user id. A user can hold at most
    /// one reaction at a time.</summary>
    Dictionary<Guid, ReactionType> Reactions { get; }

    /// <summary>Sets the given user's reaction. Setting the same reaction the user
    /// already has removes it (toggle off); setting a different reaction replaces it.</summary>
    void SetReaction(Guid userId, ReactionType reaction);
}