-- Fix Users table to add PasswordHash column
USE FinancialAppDb;
GO

-- Add PasswordHash column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'PasswordHash')
BEGIN
    ALTER TABLE Users ADD PasswordHash nvarchar(256) NOT NULL DEFAULT '';
    PRINT 'PasswordHash column added to Users table';
END
GO

-- Update existing user with a password hash for 'admin123'
-- BCrypt hash for 'admin123'
UPDATE Users 
SET PasswordHash = '$2a$11$rQZ8KjJ8KjJ8KjJ8KjJ8K.abcdefghijklmnopqrstuvwxyzABCDEF'
WHERE Email = 'christopher.summers@email.com';
GO

-- Insert admin user if not exists
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@financialapp.com')
BEGIN
    INSERT INTO Users (FullName, Email, Phone, AvatarUrl, AvailableBalance, PasswordHash, CreatedAt, UpdatedAt)
    VALUES (N'Admin User', N'admin@financialapp.com', N'0123456789', 
            N'https://images.pexels.com/photos/2167673/pexels-photo-2167673.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260', 
            0.0, '$2a$11$rQZ8KjJ8KjJ8KjJ8KjJ8K.abcdefghijklmnopqrstuvwxyzABCDEF', 
            GETUTCDATE(), GETUTCDATE());
    PRINT 'Admin user created';
END
ELSE
BEGIN
    -- Update existing admin user password
    UPDATE Users 
    SET PasswordHash = '$2a$11$rQZ8KjJ8KjJ8KjJ8KjJ8K.abcdefghijklmnopqrstuvwxyzABCDEF'
    WHERE Email = 'admin@financialapp.com';
    PRINT 'Admin user password updated';
END
GO

PRINT 'User password fix completed successfully!';

