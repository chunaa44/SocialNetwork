using Microsoft.Data.Sqlite;
using SocialPlatformLibrary.DTO;
using SocialPlatformLibrary.Interfaces;
using System;
using System.Collections.Generic;

namespace SocialPlatformLibrary.Repositories;

/// <summary>
/// SQLite implementation of IUserRepo using ADO.NET.
/// Follow relationships are stored in a separate Follows join table.
/// </summary>
public class UserRepoSQLite : IUserRepo
{
    private readonly SqliteConnection _connection;

    public UserRepoSQLite(SqliteConnection connection)
    {
        _connection = connection;
    }

    public User CreateUser(UserDTO dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password
        };

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Users (Id, Name, Email, Password)
            VALUES ($id, $name, $email, $password);
            """;
        cmd.Parameters.AddWithValue("$id", user.Id.ToString());
        cmd.Parameters.AddWithValue("$name", user.Name);
        cmd.Parameters.AddWithValue("$email", user.Email);
        cmd.Parameters.AddWithValue("$password", user.Password);
        cmd.ExecuteNonQuery();

        return user;
    }

    public User? GetUserById(Guid id)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Email, Password FROM Users WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id.ToString());

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return MapUser(reader);
    }

    public List<User> GetAllUsers()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Email, Password FROM Users;";

        var list = new List<User>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapUser(reader));

        return list;
    }

    public User? UpdateUserById(Guid id, string name, string email)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "UPDATE Users SET Name = $name, Email = $email WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$name", name);
        cmd.Parameters.AddWithValue("$email", email);
        cmd.Parameters.AddWithValue("$id", id.ToString());

        int affected = cmd.ExecuteNonQuery();
        if (affected == 0) return null;

        return GetUserById(id);
    }

    public bool RemoveUserById(Guid id)
    {
        // everything related to deleted entity is cleaned up by
        // sqlite cascading deletion.
        // simply deleting the entity is enough
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Users WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id.ToString());
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool FollowUser(Guid followerId, Guid followeeId)
    {
        using var checkCmd = _connection.CreateCommand();
        checkCmd.CommandText = """
            SELECT COUNT(1) FROM Follows WHERE FollowerId = $followerId AND FolloweeId = $followeeId;
            """;
        checkCmd.Parameters.AddWithValue("$followerId", followerId.ToString());
        checkCmd.Parameters.AddWithValue("$followeeId", followeeId.ToString());
        var alreadyFollowing = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

        // false if the relationship already existed
        if (alreadyFollowing) return false;

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Follows (FollowerId, FolloweeId) VALUES ($followerId, $followeeId);
            """;
        cmd.Parameters.AddWithValue("$followerId", followerId.ToString());
        cmd.Parameters.AddWithValue("$followeeId", followeeId.ToString());
        cmd.ExecuteNonQuery();

        return true;
    }

    public bool UnfollowUser(Guid followerId, Guid followeeId)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            DELETE FROM Follows WHERE FollowerId = $followerId AND FolloweeId = $followeeId;
            """;
        cmd.Parameters.AddWithValue("$followerId", followerId.ToString());
        cmd.Parameters.AddWithValue("$followeeId", followeeId.ToString());

        return cmd.ExecuteNonQuery() > 0;
    }

    public List<User> GetFollowers(Guid userId)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            SELECT u.Id, u.Name, u.Email, u.Password
            FROM Users u
            JOIN Follows f ON f.FollowerId = u.Id
            WHERE f.FolloweeId = $userId;
            """;
        cmd.Parameters.AddWithValue("$userId", userId.ToString());

        var list = new List<User>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapUser(reader));

        return list;
    }

    public List<User> GetFollowing(Guid userId)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            SELECT u.Id, u.Name, u.Email, u.Password
            FROM Users u
            JOIN Follows f ON f.FolloweeId = u.Id
            WHERE f.FollowerId = $userId;
            """;
        cmd.Parameters.AddWithValue("$userId", userId.ToString());

        var list = new List<User>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapUser(reader));

        return list;
    }

    private static User MapUser(SqliteDataReader reader) => new()
    {
        Id = Guid.Parse(reader.GetString(0)),
        Name = reader.GetString(1),
        Email = reader.GetString(2),
        Password = reader.GetString(3)
    };
}