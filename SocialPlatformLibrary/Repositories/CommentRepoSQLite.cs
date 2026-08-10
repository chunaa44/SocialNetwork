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
        // everything related to deleted entity is cleaned up by
        // sqlite cascading deletion.
        // simply deleting the entity is enough
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Comments WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id.ToString());
        return cmd.ExecuteNonQuery() > 0;
    }

    public void SetReaction(Guid commentId, Guid userId, ReactionType reaction)
    {
        using var checkCmd = _connection.CreateCommand();
        checkCmd.CommandText = """
            SELECT Type FROM Reactions WHERE PostId = $postId AND UserId = $userId;
            """;
        checkCmd.Parameters.AddWithValue("$postId", commentId.ToString());
        checkCmd.Parameters.AddWithValue("$userId", userId.ToString());
        var existing = checkCmd.ExecuteScalar() as string;

        using var cmd = _connection.CreateCommand();
        if (existing == reaction.ToString())
        {
            cmd.CommandText = "DELETE FROM Reactions WHERE PostId = $postId AND UserId = $userId;";
        }
        else if (existing != null)
        {
            cmd.CommandText = "UPDATE Reactions SET Type = $type WHERE PostId = $postId AND UserId = $userId;";
            cmd.Parameters.AddWithValue("$type", reaction.ToString());
        }
        else
        {
            cmd.CommandText = "INSERT INTO Reactions (PostId, UserId, Type) VALUES ($postId, $userId, $type);";
            cmd.Parameters.AddWithValue("$type", reaction.ToString());
        }

        cmd.Parameters.AddWithValue("$postId", commentId.ToString());
        cmd.Parameters.AddWithValue("$userId", userId.ToString());
        cmd.ExecuteNonQuery();
    }

    public Dictionary<Guid, ReactionType> GetReactions(Guid commentId)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT UserId, Type FROM Reactions WHERE PostId = $postId;";
        cmd.Parameters.AddWithValue("$postId", commentId.ToString());

        var reactions = new Dictionary<Guid, ReactionType>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            reactions[Guid.Parse(reader.GetString(0))] = Enum.Parse<ReactionType>(reader.GetString(1));

        return reactions;
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