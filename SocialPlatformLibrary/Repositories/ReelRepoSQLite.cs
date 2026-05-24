using Microsoft.Data.Sqlite;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Repositories;

/// <summary>
/// SQLite implementation of IReelRepo using ADO.NET.
/// </summary>
public class ReelRepoSQLite : IReelRepo
{
    private readonly SqliteConnection _connection;

    public ReelRepoSQLite(SqliteConnection connection)
    {
        _connection = connection;
    }

    public Reel CreateReel(ReelDTO dto)
    {
        var reel = new Reel
        {
            AuthorId = dto.Author.Id,
            Content = dto.Content
        };

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Reels (Id, AuthorId, Content, Timestamp)
            VALUES ($id, $authorId, $content, $timestamp);
            """;
        cmd.Parameters.AddWithValue("$id", reel.Id.ToString());
        cmd.Parameters.AddWithValue("$authorId", reel.AuthorId.ToString());
        cmd.Parameters.AddWithValue("$content", reel.Content);
        cmd.Parameters.AddWithValue("$timestamp", reel.Timestamp.ToString("O"));
        cmd.ExecuteNonQuery();

        return reel;
    }

    public Reel? GetReelById(Guid id)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT Id, AuthorId, Content, Timestamp FROM Reels WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id.ToString());

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return MapReel(reader);
    }

    public List<Reel> GetAllReels()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT Id, AuthorId, Content, Timestamp FROM Reels;";

        var list = new List<Reel>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapReel(reader));

        return list;
    }

    public Reel? UpdateReelById(Guid id, string newContent)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "UPDATE Reels SET Content = $content WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$content", newContent);
        cmd.Parameters.AddWithValue("$id", id.ToString());

        int affected = cmd.ExecuteNonQuery();
        if (affected == 0) return null;

        return GetReelById(id);
    }

    public bool RemoveReelById(Guid id)
    {
        using var likesCmd = _connection.CreateCommand();
        likesCmd.CommandText = "DELETE FROM Likes WHERE PostId = $id;";
        likesCmd.Parameters.AddWithValue("$id", id.ToString());
        likesCmd.ExecuteNonQuery();

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Reels WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id.ToString());
        return cmd.ExecuteNonQuery() > 0;
    }

    public void ToggleLike(Guid reelId, Guid userId)
    {
        using var checkCmd = _connection.CreateCommand();
        checkCmd.CommandText = """
            SELECT COUNT(1) FROM Likes WHERE PostId = $postId AND UserId = $userId;
            """;
        checkCmd.Parameters.AddWithValue("$postId", reelId.ToString());
        checkCmd.Parameters.AddWithValue("$userId", userId.ToString());
        var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

        using var cmd = _connection.CreateCommand();
        if (exists)
            cmd.CommandText = "DELETE FROM Likes WHERE PostId = $postId AND UserId = $userId;";
        else
            cmd.CommandText = "INSERT INTO Likes (PostId, UserId) VALUES ($postId, $userId);";

        cmd.Parameters.AddWithValue("$postId", reelId.ToString());
        cmd.Parameters.AddWithValue("$userId", userId.ToString());
        cmd.ExecuteNonQuery();
    }

    public HashSet<Guid> GetLikes(Guid reelId)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT UserId FROM Likes WHERE PostId = $postId;";
        cmd.Parameters.AddWithValue("$postId", reelId.ToString());

        var likes = new HashSet<Guid>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            likes.Add(Guid.Parse(reader.GetString(0)));

        return likes;
    }

    private static Reel MapReel(SqliteDataReader reader) => new()
    {
        Id = Guid.Parse(reader.GetString(0)),
        AuthorId = Guid.Parse(reader.GetString(1)),
        Content = reader.GetString(2),
        Timestamp = DateTime.Parse(reader.GetString(3))
    };
}
