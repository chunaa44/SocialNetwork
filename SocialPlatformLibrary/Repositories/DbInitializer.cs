using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

namespace SocialPlatformLibrary.Repositories;

/// <summary>
/// Runs once at app startup to ensure all tables exist in the SQLite database.
/// Safe to call multiple times — uses CREATE TABLE IF NOT EXISTS.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Creates Photos, Reels, Stories, Comments, and Likes tables if they don't exist.
    /// </summary>
    /// <param name="connection">An already-opened SQLite connection.</param>
    public static void Initialize(SqliteConnection connection)
    {
        using var cmd = connection.CreateCommand();

        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS Photos (
                Id        TEXT NOT NULL PRIMARY KEY,
                AuthorId  TEXT NOT NULL,
                Content   TEXT NOT NULL,
                PhotoUrl  TEXT NOT NULL,
                Timestamp TEXT NOT NULL
            );
 
            CREATE TABLE IF NOT EXISTS Reels (
                Id        TEXT NOT NULL PRIMARY KEY,
                AuthorId  TEXT NOT NULL,
                Content   TEXT NOT NULL,
                Timestamp TEXT NOT NULL
            );
 
            CREATE TABLE IF NOT EXISTS Stories (
                Id        TEXT NOT NULL PRIMARY KEY,
                AuthorId  TEXT NOT NULL,
                Content   TEXT NOT NULL,
                ExpiresAt TEXT NOT NULL,
                Timestamp TEXT NOT NULL
            );
 
            CREATE TABLE IF NOT EXISTS Comments (
                Id        TEXT NOT NULL PRIMARY KEY,
                AuthorId  TEXT NOT NULL,
                ParentId  TEXT NOT NULL,
                Content   TEXT NOT NULL,
                Timestamp TEXT NOT NULL
            );
 
            CREATE TABLE IF NOT EXISTS Likes (
                PostId    TEXT NOT NULL,
                UserId    TEXT NOT NULL,
                PRIMARY KEY (PostId, UserId)
            );
            """;

        cmd.ExecuteNonQuery();
    }
}
