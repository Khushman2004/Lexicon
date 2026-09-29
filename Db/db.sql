/* =========================================================
   LEXICON DATABASE
   Authentication Schema
   ========================================================= */

IF DB_ID('Lexicon') IS NULL
BEGIN
    CREATE DATABASE Lexicon;
END
GO

USE Lexicon;
GO


/* =========================================================
   USERS
   Stores the minimum information required for authentication.
   ========================================================= */

IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_Users
            PRIMARY KEY
            DEFAULT NEWID(),

        Username NVARCHAR(50) NOT NULL,

        Email NVARCHAR(320) NOT NULL,

        PasswordHash NVARCHAR(500) NOT NULL,

        IsActive BIT NOT NULL
            CONSTRAINT DF_Users_IsActive
            DEFAULT 1,

        CreatedAtUtc DATETIME2 NOT NULL
            CONSTRAINT DF_Users_CreatedAtUtc
            DEFAULT SYSUTCDATETIME()
    );
END
GO


/* =========================================================
   UNIQUE CONSTRAINTS
   ========================================================= */

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_Users_Username'
      AND object_id = OBJECT_ID('dbo.Users')
)
BEGIN
    CREATE UNIQUE INDEX UX_Users_Username
        ON dbo.Users(Username);
END
GO


IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_Users_Email'
      AND object_id = OBJECT_ID('dbo.Users')
)
BEGIN
    CREATE UNIQUE INDEX UX_Users_Email
        ON dbo.Users(Email);
END
GO


/* =========================================================
   AUTH SESSIONS
   Stores server-side authentication state.
   ========================================================= */

IF OBJECT_ID('dbo.AuthSessions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuthSessions
    (
        Id UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_AuthSessions
            PRIMARY KEY
            DEFAULT NEWID(),

        UserId UNIQUEIDENTIFIER NOT NULL,

        CreatedAtUtc DATETIME2 NOT NULL
            CONSTRAINT DF_AuthSessions_CreatedAtUtc
            DEFAULT SYSUTCDATETIME(),

        ExpiresAtUtc DATETIME2 NOT NULL,

        RevokedAtUtc DATETIME2 NULL,

        LastUsedAtUtc DATETIME2 NULL,

        CONSTRAINT FK_AuthSessions_Users
            FOREIGN KEY (UserId)
            REFERENCES dbo.Users(Id)
    );
END
GO


/* =========================================================
   INDEXES
   ========================================================= */

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_AuthSessions_UserId'
      AND object_id = OBJECT_ID('dbo.AuthSessions')
)
BEGIN
    CREATE INDEX IX_AuthSessions_UserId
        ON dbo.AuthSessions(UserId);
END
GO