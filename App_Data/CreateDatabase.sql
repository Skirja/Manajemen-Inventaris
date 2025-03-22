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

-- Create Categories table
CREATE TABLE [dbo].[Categories] (
    [CategoryID] INT IDENTITY(1, 1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(255) NULL,
    [CreatedBy] INT NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY CLUSTERED ([CategoryID] ASC),
    FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserID])
);

-- Insert default categories
INSERT INTO [dbo].[Categories] ([Name], [Description], [CreatedBy])
VALUES ('Electronics', 'Electronic devices and components', 1);

INSERT INTO [dbo].[Categories] ([Name], [Description], [CreatedBy])
VALUES ('Office Supplies', 'Office stationery and supplies', 1);

INSERT INTO [dbo].[Categories] ([Name], [Description], [CreatedBy])
VALUES ('Furniture', 'Office furniture and fixtures', 1);

-- Create Items table
CREATE TABLE [dbo].[Items] (
    [ItemID] INT IDENTITY(1, 1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [CategoryID] INT NOT NULL,
    [Quantity] INT NOT NULL DEFAULT 0,
    [ImagePath] NVARCHAR(255) NULL,
    [AITags] NVARCHAR(MAX) NULL,
    [CreatedBy] INT NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [LastModifiedBy] INT NOT NULL,
    [LastModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY CLUSTERED ([ItemID] ASC),
    FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Categories] ([CategoryID]),
    FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserID]),
    FOREIGN KEY ([LastModifiedBy]) REFERENCES [dbo].[Users] ([UserID])
);

-- Create ItemHistory table for tracking inventory changes
CREATE TABLE [dbo].[ItemHistory] (
    [HistoryID] INT IDENTITY(1, 1) NOT NULL,
    [ItemID] INT NOT NULL,
    [ChangeType] NVARCHAR(50) NOT NULL, -- 'StockIn', 'StockOut', 'Created', 'Updated', 'Deleted'
    [QuantityChanged] INT NOT NULL,
    [PreviousQuantity] INT NOT NULL,
    [NewQuantity] INT NOT NULL,
    [Notes] NVARCHAR(255) NULL,
    [ChangedBy] INT NOT NULL,
    [ChangedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY CLUSTERED ([HistoryID] ASC),
    FOREIGN KEY ([ItemID]) REFERENCES [dbo].[Items] ([ItemID]),
    FOREIGN KEY ([ChangedBy]) REFERENCES [dbo].[Users] ([UserID])
);

-- Insert sample items
INSERT INTO [dbo].[Items] ([Name], [Description], [CategoryID], [Quantity], [CreatedBy], [LastModifiedBy])
VALUES ('Laptop', 'Dell XPS 13 Laptop', 1, 10, 1, 1);

INSERT INTO [dbo].[Items] ([Name], [Description], [CategoryID], [Quantity], [CreatedBy], [LastModifiedBy])
VALUES ('Notebook', 'Spiral bound notebook', 2, 50, 1, 1);

INSERT INTO [dbo].[Items] ([Name], [Description], [CategoryID], [Quantity], [CreatedBy], [LastModifiedBy])
VALUES ('Office Chair', 'Ergonomic office chair', 3, 5, 1, 1);

-- Insert sample history records
INSERT INTO [dbo].[ItemHistory] ([ItemID], [ChangeType], [QuantityChanged], [PreviousQuantity], [NewQuantity], [Notes], [ChangedBy])
VALUES (1, 'StockIn', 10, 0, 10, 'Initial stock', 1);

INSERT INTO [dbo].[ItemHistory] ([ItemID], [ChangeType], [QuantityChanged], [PreviousQuantity], [NewQuantity], [Notes], [ChangedBy])
VALUES (2, 'StockIn', 50, 0, 50, 'Initial stock', 1);

INSERT INTO [dbo].[ItemHistory] ([ItemID], [ChangeType], [QuantityChanged], [PreviousQuantity], [NewQuantity], [Notes], [ChangedBy])
VALUES (3, 'StockIn', 5, 0, 5, 'Initial stock', 1); 