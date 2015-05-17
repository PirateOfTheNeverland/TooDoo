-- =========================================
-- Create table template SQL Azure Database 
-- =========================================

IF OBJECT_ID('dbo.FriendEntries', 'U') IS NOT NULL
  DROP TABLE dbo.FriendEntries
GO

CREATE TABLE dbo.FriendEntries
(
	[FriendId] INT         IDENTITY NOT NULL,
    [Owner]       NVARCHAR (80)   NOT NULL,
    [Name]       NVARCHAR (80)   NULL,
    [Notes]       NVARCHAR (1000) NULL,
    [IsDeleted]      BIT         NOT NULL,
	[IsBlocked]      BIT         NOT NULL,
    PRIMARY KEY CLUSTERED ([FriendId] ASC)
)
GO