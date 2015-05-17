-- =========================================
-- Create table template SQL Azure Database 
-- =========================================

IF OBJECT_ID('dbo.FixItTasks', 'U') IS NOT NULL
  DROP TABLE dbo.FixItTasks
GO

CREATE TABLE dbo.FixItTasks
(
	[FixItTaskId] INT         IDENTITY NOT NULL,
    [CreatedBy]   NVARCHAR (80)   NOT NULL,
    [Owner]       NVARCHAR (80)   NOT NULL,
    [Title]       NVARCHAR (80)   NULL,
    [Notes]       NVARCHAR (1000) NULL,
    [PhotoUrl]    NVARCHAR (200)  NULL,
    [IsDone]      BIT         NOT NULL,
    PRIMARY KEY CLUSTERED ([FixItTaskId] ASC)
)
GO
