-- Create ChatMessages table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ChatMessages' AND xtype='U')
BEGIN
    CREATE TABLE [ChatMessages] (
        [Id] int NOT NULL IDENTITY(1,1),
        [UserId] int NOT NULL,
        [Message] nvarchar(1000) NOT NULL,
        [Response] nvarchar(2000) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [MessageType] nvarchar(20) NOT NULL DEFAULT 'user',
        CONSTRAINT [PK_ChatMessages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ChatMessages_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_ChatMessages_UserId_CreatedAt] ON [ChatMessages] ([UserId], [CreatedAt]);
    
    PRINT 'ChatMessages table created successfully';
END
ELSE
BEGIN
    PRINT 'ChatMessages table already exists';
END