using Microsoft.Data.Sqlite;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Repositories;

/// <summary>
/// SQLite implementation of IStoryRepo using ADO.NET.
/// Story has an ExpiresAt column since stories expire after 24 hours.
/// </summary>
public class StoryRepoSQLite : IStoryRepo
{
    private readonly SqliteConnection _connection;

    public StoryRepoSQLite(SqliteConnection connection)
    {
        _connection = connection;
    }

    public Story CreateStory(StoryDTO dto)
    {
        var story = new Story
        {
            AuthorId = dto.Author.Id,
            Content = dto.Content
        };

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Stories (Id, AuthorId, Content, ExpiresAt, Timestamp)
            VALUES ($id, $authorId, $content, $expiresAt, $timestamp);
            """;
        cmd.Parameters.AddWithValue("$id", story.Id.ToString());
        cmd.Parameters.AddWithValue("$authorId", story.AuthorId.ToString());
        cmd.Parameters.AddWithValue("$content", story.Content);
        cmd.Parameters.AddWithValue("$expiresAt", story.ExpiresAt.ToString("O"));
        cmd.Parameters.AddWithValue("$timestamp", story.Timestamp.ToString("O"));
        cmd.ExecuteNonQuery();

        return story;
    }

    public Story? GetStoryById(Guid id)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            SELECT Id, AuthorId, Content, ExpiresAt, Timestamp
            FROM Stories WHERE Id = $id;
            """;
        cmd.Parameters.AddWithValue("$id", id.ToString());

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return MapStory(reader);
    }

    public List<Story> GetAllStories()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT Id, AuthorId, Content, ExpiresAt, Timestamp FROM Stories;";

        var list = new List<Story>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapStory(reader));

        return list;
    }

    public Story? UpdateStoryById(Guid id, string newContent)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "UPDATE Stories SET Content = $content WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$content", newContent);
        cmd.Parameters.AddWithValue("$id", id.ToString());

        int affected = cmd.ExecuteNonQuery();
        if (affected == 0) return null;

        return GetStoryById(id);
    }

    public bool RemoveStoryById(Guid id)
    {
        using var likesCmd = _connection.CreateCommand();
        likesCmd.CommandText = "DELETE FROM Likes WHERE PostId = $id;";
        likesCmd.Parameters.AddWithValue("$id", id.ToString());
        likesCmd.ExecuteNonQuery();

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Stories WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id.ToString());
        return cmd.ExecuteNonQuery() > 0;
    }

    public void ToggleLike(Guid storyId, Guid userId)
    {
        using var checkCmd = _connection.CreateCommand();
        checkCmd.CommandText = """
            SELECT COUNT(1) FROM Likes WHERE PostId = $postId AND UserId = $userId;
            """;
        checkCmd.Parameters.AddWithValue("$postId", storyId.ToString());
        checkCmd.Parameters.AddWithValue("$userId", userId.ToString());
        var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

        using var cmd = _connection.CreateCommand();
        if (exists)
            cmd.CommandText = "DELETE FROM Likes WHERE PostId = $postId AND UserId = $userId;";
        else
            cmd.CommandText = "INSERT INTO Likes (PostId, UserId) VALUES ($postId, $userId);";

        cmd.Parameters.AddWithValue("$postId", storyId.ToString());
        cmd.Parameters.AddWithValue("$userId", userId.ToString());
        cmd.ExecuteNonQuery();
    }

    public HashSet<Guid> GetLikes(Guid storyId)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT UserId FROM Likes WHERE PostId = $postId;";
        cmd.Parameters.AddWithValue("$postId", storyId.ToString());

        var likes = new HashSet<Guid>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            likes.Add(Guid.Parse(reader.GetString(0)));

        return likes;
    }

    private static Story MapStory(SqliteDataReader reader) => new()
    {
        Id = Guid.Parse(reader.GetString(0)),
        AuthorId = Guid.Parse(reader.GetString(1)),
        Content = reader.GetString(2),
        ExpiresAt = DateTime.Parse(reader.GetString(3)),
        Timestamp = DateTime.Parse(reader.GetString(4))
    };
}
