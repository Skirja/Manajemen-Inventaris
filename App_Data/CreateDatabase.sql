-- Create Users table
CREATE TABLE [dbo].[Users] (
    [UserID]   INT           IDENTITY (1, 1) NOT NULL,
    [Username] NVARCHAR (50) NOT NULL,
    [Password] NVARCHAR (50) NOT NULL,
    [Email]    NVARCHAR (100) NULL,
    [Company]  NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([UserID] ASC)
);

-- Insert default admin user
INSERT INTO [dbo].[Users] ([Username], [Password], [Email], [Company])
VALUES ('admin', 'admin123', 'admin@example.com', 'Admin Company');

-- Insert default regular user
INSERT INTO [dbo].[Users] ([Username], [Password], [Email], [Company])
VALUES ('user', 'user123', 'user@example.com', 'User Company'); 