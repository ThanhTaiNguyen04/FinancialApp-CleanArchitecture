-- Update user role to Admin for admin@financialapp.com
UPDATE "Users" 
SET "Role" = 'Admin' 
WHERE "Email" = 'admin@financialapp.com';

-- Verify the update
SELECT "Id", "Email", "FullName", "Role" 
FROM "Users" 
WHERE "Email" = 'admin@financialapp.com';

-- Show all admin users
SELECT "Id", "Email", "FullName", "Role" 
FROM "Users" 
WHERE "Role" = 'Admin';
