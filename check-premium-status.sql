-- Check current user subscription status
SELECT 
    Id,
    Email,
    FullName,
    SubscriptionType,
    PremiumExpiry,
    CreatedAt,
    CASE 
        WHEN SubscriptionType = 'Premium' AND PremiumExpiry > GETDATE() THEN 'Active Premium'
        WHEN SubscriptionType = 'Premium' AND PremiumExpiry <= GETDATE() THEN 'Expired Premium'
        ELSE 'Free User'
    END as Status
FROM Users
ORDER BY CreatedAt DESC;

-- Update a user to Premium for testing (replace Email)
-- UPDATE Users 
-- SET SubscriptionType = 'Premium', 
--     PremiumExpiry = DATEADD(month, 1, GETDATE())
-- WHERE Email = 'your-test-email@example.com';