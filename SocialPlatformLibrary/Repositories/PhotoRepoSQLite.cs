using Microsoft.Data.Sqlite;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using SocialPlatformLibrary.Posts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPlatformLibrary.Repositories;

/// <summary>
/// SQLite implementation of IPhotoRepo using ADO.NET.
/// </summary>
public class PhotoRepoSQLite : IPhotoRepo
{
    private readonly SqliteConnection _connection;

    public PhotoRepoSQLite(SqliteConnection connection)
    {
        _connection = connection;
    }

    public Photo CreatePhoto(PhotoDTO dto)
    {
        var photo = new Photo
        {
            AuthorId = dto.Author.Id,
            Content = dto.Content,
            PhotoUrl = dto.PhotoUrl
        };

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Photos (Id, AuthorId, Content, PhotoUrl, Timestamp)
            VALUES ($id, $authorId, $content, $photoUrl, $timestamp);
            """;
        cmd.Parameters.AddWithValue("$id", photo.Id.ToString());
        cmd.Parameters.AddWithValue("$authorId", photo.AuthorId.ToString());
        cmd.Parameters.AddWithValue("$content", photo.Content);
        cmd.Parameters.AddWithValue("$photoUrl", photo.PhotoUrl);
        cmd.Parameters.AddWithValue("$timestamp", photo.Timestamp.ToString("O"));
        cmd.ExecuteNonQuery();

        return photo;
    }

    public Photo? GetPhotoById(Guid id)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT Id, AuthorId, Content, PhotoUrl, Timestamp FROM Photos WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id.ToString());

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return MapPhoto(reader);
    }

    public List<Photo> GetAllPhotos()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT Id, AuthorId, Content, PhotoUrl, Timestamp FROM Photos;";

        var list = new List<Photo>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapPhoto(reader));

        return list;
    }

    public Photo? UpdatePhotoById(Guid id, string newContent, string newPhotoUrl)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            UPDATE Photos SET Content = $content, PhotoUrl = $photoUrl
            WHERE Id = $id;
            """;
        cmd.Parameters.AddWithValue("$content", newContent);
        cmd.Parameters.AddWithValue("$photoUrl", newPhotoUrl);
        cmd.Parameters.AddWithValue("$id", id.ToString());

        // ExecuteNonQuery returns number of rows affected
        // if 0, the photo didn't exist
        int affected = cmd.ExecuteNonQuery();
        if (affected == 0) return null;

        return GetPhotoById(id);
    }

    public bool RemovePhotoById(Guid id)
    {
        // Delete likes for this post first
        using var likesCmd = _connection.CreateCommand();
        likesCmd.CommandText = "DELETE FROM Likes WHERE PostId = $id;";
        likesCmd.Parameters.AddWithValue("$id", id.ToString());
        likesCmd.ExecuteNonQuery();

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Photos WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id.ToString());
        return cmd.ExecuteNonQuery() > 0;
    }

    public void ToggleLike(Guid photoId, Guid userId)
    {
        // Check if like already exists
        using var checkCmd = _connection.CreateCommand();
        checkCmd.CommandText = """
            SELECT COUNT(1) FROM Likes WHERE PostId = $postId AND UserId = $userId;
            """;
        checkCmd.Parameters.AddWithValue("$postId", photoId.ToString());
        checkCmd.Parameters.AddWithValue("$userId", userId.ToString());
        var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

        using var cmd = _connection.CreateCommand();
        if (exists)
            cmd.CommandText = "DELETE FROM Likes WHERE PostId = $postId AND UserId = $userId;";
        else
            cmd.CommandText = "INSERT INTO Likes (PostId, UserId) VALUES ($postId, $userId);";

        cmd.Parameters.AddWithValue("$postId", photoId.ToString());
        cmd.Parameters.AddWithValue("$userId", userId.ToString());
        cmd.ExecuteNonQuery();
    }

    public HashSet<Guid> GetLikes(Guid photoId)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT UserId FROM Likes WHERE PostId = $postId;";
        cmd.Parameters.AddWithValue("$postId", photoId.ToString());

        var likes = new HashSet<Guid>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            likes.Add(Guid.Parse(reader.GetString(0)));

        return likes;
    }

    // Maps a row from the reader into a Photo object
    // reader.GetString(0) = first column, GetString(1) = second column, etc.
    private static Photo MapPhoto(SqliteDataReader reader) => new()
    {
        Id = Guid.Parse(reader.GetString(0)),
        AuthorId = Guid.Parse(reader.GetString(1)),
        Content = reader.GetString(2),
        PhotoUrl = reader.GetString(3),
        Timestamp = DateTime.Parse(reader.GetString(4))
    };
}
