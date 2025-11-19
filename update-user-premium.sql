-- Update user a@gmail.com to Premium status
-- Run this in Supabase SQL Editor

UPDATE "Users"
SET 
    "IsPremium" = true,
    "SubscriptionType" = 'Premium',
    "PremiumExpiry" = NOW() + INTERVAL '1 month'
WHERE "Email" = 'a@gmail.com';

-- Verify the update
SELECT 
    "Id",
    "Email", 
    "FullName",
    "IsPremium",
    "SubscriptionType",
    "PremiumExpiry"
FROM "Users"
WHERE "Email" = 'a@gmail.com';
