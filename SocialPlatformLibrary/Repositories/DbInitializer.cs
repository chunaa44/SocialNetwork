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
    /// Creates Users, Follows, Photos, Reels, Stories, Comments, Reactions, Bookmarks, and Views
    /// tables if they don't exist, wires up foreign keys, and adds cleanup triggers for the
    /// polymorphic Reactions/Comments relations so deleting a post cascades correctly.
    /// </summary>
    /// <param name="connection">An already-opened SQLite connection.</param>
    public static void Initialize(SqliteConnection connection)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS Users (
                Id       TEXT NOT NULL PRIMARY KEY,
                Name     TEXT NOT NULL,
                Email    TEXT NOT NULL,
                Password TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Follows (
                FollowerId TEXT NOT NULL,
                FolloweeId TEXT NOT NULL,
                PRIMARY KEY (FollowerId, FolloweeId),
                FOREIGN KEY (FollowerId) REFERENCES Users(Id) ON DELETE CASCADE,
                FOREIGN KEY (FolloweeId) REFERENCES Users(Id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Photos (
                Id        TEXT NOT NULL PRIMARY KEY,
                AuthorId  TEXT NOT NULL,
                Content   TEXT NOT NULL,
                PhotoUrl  TEXT NOT NULL,
                Timestamp TEXT NOT NULL,
                FOREIGN KEY (AuthorId) REFERENCES Users(Id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Reels (
                Id        TEXT NOT NULL PRIMARY KEY,
                AuthorId  TEXT NOT NULL,
                Content   TEXT NOT NULL,
                Timestamp TEXT NOT NULL,
                FOREIGN KEY (AuthorId) REFERENCES Users(Id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Stories (
                Id        TEXT NOT NULL PRIMARY KEY,
                AuthorId  TEXT NOT NULL,
                Content   TEXT NOT NULL,
                ExpiresAt TEXT NOT NULL,
                Timestamp TEXT NOT NULL,
                FOREIGN KEY (AuthorId) REFERENCES Users(Id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Comments (
                Id        TEXT NOT NULL PRIMARY KEY,
                AuthorId  TEXT NOT NULL,
                ParentId  TEXT NOT NULL,
                Content   TEXT NOT NULL,
                Timestamp TEXT NOT NULL,
                FOREIGN KEY (AuthorId) REFERENCES Users(Id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Reactions (
                PostId TEXT NOT NULL,
                UserId TEXT NOT NULL,
                Type   TEXT NOT NULL,
                PRIMARY KEY (PostId, UserId),
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Bookmarks (
                PostId TEXT NOT NULL,
                UserId TEXT NOT NULL,
                PRIMARY KEY (PostId, UserId),
                FOREIGN KEY (PostId) REFERENCES Photos(Id) ON DELETE CASCADE,
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Views (
                PostId TEXT NOT NULL,
                UserId TEXT NOT NULL,
                PRIMARY KEY (PostId, UserId),
                FOREIGN KEY (PostId) REFERENCES Stories(Id) ON DELETE CASCADE,
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );

            -- Reactions/Comments can belong to more than one kind of post, so SQLite can't
            -- enforce a single FK for PostId/ParentId. These triggers do that cleanup
            -- instead, and they also fire when the row above was itself removed by a
            -- cascading FK delete (e.g. deleting a User cascades to Photos, which fires
            -- the trigger below).
            CREATE TRIGGER IF NOT EXISTS trg_photos_delete_cleanup
            AFTER DELETE ON Photos
            BEGIN
                DELETE FROM Reactions WHERE PostId = OLD.Id;
                DELETE FROM Comments WHERE ParentId = OLD.Id;
            END;

            CREATE TRIGGER IF NOT EXISTS trg_reels_delete_cleanup
            AFTER DELETE ON Reels
            BEGIN
                DELETE FROM Reactions WHERE PostId = OLD.Id;
                DELETE FROM Comments WHERE ParentId = OLD.Id;
            END;

            CREATE TRIGGER IF NOT EXISTS trg_stories_delete_cleanup
            AFTER DELETE ON Stories
            BEGIN
                DELETE FROM Reactions WHERE PostId = OLD.Id;
            END;

            CREATE TRIGGER IF NOT EXISTS trg_comments_delete_cleanup
            AFTER DELETE ON Comments
            BEGIN
                DELETE FROM Reactions WHERE PostId = OLD.Id;
            END;
            """;
        cmd.ExecuteNonQuery();
    }
}