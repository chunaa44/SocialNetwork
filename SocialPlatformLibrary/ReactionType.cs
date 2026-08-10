namespace SocialPlatformLibrary;

/// <summary>
/// The kinds of reaction a user can leave on a post. A user can only hold
/// one reaction of any type on a given post at a time.
/// </summary>
public enum ReactionType
{
    Like,
    Love,
    Haha,
    Wow,
    Sad,
    Angry
}