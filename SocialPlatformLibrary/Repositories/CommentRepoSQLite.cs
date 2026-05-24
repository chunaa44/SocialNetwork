using Microsoft.Data.Sqlite;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Repositories;

/// <summary>
/// SQLite implementation of ICommentRepo using ADO.NET.
/// Comments have a ParentId pointing to the post they belong to.
/// </summary>
public class CommentRepoSQLite : ICommentRepo
{
    private readonly SqliteConnection _connection;

    public CommentRepoSQLite(SqliteConnection connection)
    {
        _connection = connection;
    }

    public Comment CreateComment(CommentDTO dto)
    {
        var comment = new Comment
        {
            AuthorId = dto.Author.Id,
            Content = dto.Content,
            ParentId = dto.ParentId
        };

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Comments (Id, AuthorId, ParentId, Content, Timestamp)
            VALUES ($id, $authorId, $parentId, $content, $timestamp);
            """;
        cmd.Parameters.AddWithValue("$id", comment.Id.ToString());
        cmd.Parameters.AddWithValue("$authorId", comment.AuthorId.ToString());
        cmd.Parameters.AddWithValue("$parentId", comment.ParentId.ToString());
        cmd.Parameters.AddWithValue("$content", comment.Content);
        cmd.Parameters.AddWithValue("$timestamp", comment.Timestamp.ToString("O"));
        cmd.ExecuteNonQuery();

        return comment;
    }

    public Comment? GetCommentById(Guid id)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            SELECT Id, AuthorId, ParentId, Content, Timestamp
            FROM Comments WHERE Id = $id;
            """;
        cmd.Parameters.AddWithValue("$id", id.ToString());

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return MapComment(reader);
    }

    public List<Comment> GetAllComments()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT Id, AuthorId, ParentId, Content, Timestamp FROM Comments;";

        var list = new List<Comment>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapComment(reader));

        return list;
    }

    public Comment? UpdateCommentById(Guid id, string newContent)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "UPDATE Comments SET Content = $content WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$content", newContent);
        cmd.Parameters.AddWithValue("$id", id.ToString());

        int affected = cmd.ExecuteNonQuery();
        if (affected == 0) return null;

        return GetCommentById(id);
    }

    public bool RemoveCommentById(Guid id)
    {
        using var likesCmd = _connection.CreateCommand();
        likesCmd.CommandText = "DELETE FROM Likes WHERE PostId = $id;";
        likesCmd.Parameters.AddWithValue("$id", id.ToString());
        likesCmd.ExecuteNonQuery();

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Comments WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id.ToString());
        return cmd.ExecuteNonQuery() > 0;
    }

    public void ToggleLike(Guid commentId, Guid userId)
    {
        using var checkCmd = _connection.CreateCommand();
        checkCmd.CommandText = """
            SELECT COUNT(1) FROM Likes WHERE PostId = $postId AND UserId = $userId;
            """;
        checkCmd.Parameters.AddWithValue("$postId", commentId.ToString());
        checkCmd.Parameters.AddWithValue("$userId", userId.ToString());
        var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

        using var cmd = _connection.CreateCommand();
        if (exists)
            cmd.CommandText = "DELETE FROM Likes WHERE PostId = $postId AND UserId = $userId;";
        else
            cmd.CommandText = "INSERT INTO Likes (PostId, UserId) VALUES ($postId, $userId);";

        cmd.Parameters.AddWithValue("$postId", commentId.ToString());
        cmd.Parameters.AddWithValue("$userId", userId.ToString());
        cmd.ExecuteNonQuery();
    }

    public HashSet<Guid> GetLikes(Guid commentId)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT UserId FROM Likes WHERE PostId = $postId;";
        cmd.Parameters.AddWithValue("$postId", commentId.ToString());

        var likes = new HashSet<Guid>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            likes.Add(Guid.Parse(reader.GetString(0)));

        return likes;
    }

    private static Comment MapComment(SqliteDataReader reader) => new()
    {
        Id = Guid.Parse(reader.GetString(0)),
        AuthorId = Guid.Parse(reader.GetString(1)),
        ParentId = Guid.Parse(reader.GetString(2)),
        Content = reader.GetString(3),
        Timestamp = DateTime.Parse(reader.GetString(4))
    };
}
