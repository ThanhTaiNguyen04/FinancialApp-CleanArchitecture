-- =============================================
-- FinancialApp Database Query Examples
-- Các câu truy vấn hữu ích cho ứng dụng quản lý tài chính
-- =============================================

USE FinancialAppDb;
GO

-- =============================================
-- 1. USER & PROFILE QUERIES
-- =============================================

-- Lấy thông tin user và tổng số dư
SELECT 
    u.Id, u.FullName, u.Email, u.Phone,
    u.AvailableBalance,
    COUNT(t.Id) as TotalTransactions,
    SUM(CASE WHEN t.Type = 'income' THEN t.Amount ELSE 0 END) as TotalIncome,
    SUM(CASE WHEN t.Type = 'expense' THEN t.Amount ELSE 0 END) as TotalExpenses
FROM Users u
LEFT JOIN Transactions t ON u.Id = t.UserId
WHERE u.Id = 1
GROUP BY u.Id, u.FullName, u.Email, u.Phone, u.AvailableBalance;

-- =============================================
-- 2. BUDGET ANALYSIS QUERIES  
-- =============================================

-- Budget summary với percentage used
SELECT 
    b.Id,
    c.Name as CategoryName,
    c.ColorCode,
    b.BudgetAmount,
    b.SpentAmount,
    (b.BudgetAmount - b.SpentAmount) as RemainingAmount,
    CASE 
        WHEN b.BudgetAmount > 0 THEN ROUND((b.SpentAmount / b.BudgetAmount) * 100, 2)
        ELSE 0
    END as PercentageUsed,
    CASE 
        WHEN b.SpentAmount >= b.BudgetAmount THEN 'exceeded'
        WHEN (b.SpentAmount / NULLIF(b.BudgetAmount, 0)) >= 0.95 THEN 'danger'
        WHEN (b.SpentAmount / NULLIF(b.BudgetAmount, 0)) >= 0.80 THEN 'warning'
        ELSE 'normal'
    END as AlertLevel
FROM Budgets b
INNER JOIN Categories c ON b.CategoryId = c.Id
WHERE b.UserId = 1 AND b.Month = 9 AND b.Year = 2025
ORDER BY PercentageUsed DESC;

-- Budget alerts (vượt quá 80%)
SELECT 
    b.Id,
    c.Name as CategoryName,
    b.BudgetAmount,
    b.SpentAmount,
    ROUND((b.SpentAmount / b.BudgetAmount) * 100, 2) as PercentageUsed
FROM Budgets b
INNER JOIN Categories c ON b.CategoryId = c.Id
WHERE b.UserId = 1 
    AND b.Month = MONTH(GETDATE()) 
    AND b.Year = YEAR(GETDATE())
    AND b.BudgetAmount > 0
    AND (b.SpentAmount / b.BudgetAmount) >= 0.8;

-- =============================================
-- 3. SAVING GOALS QUERIES
-- =============================================

-- Saving goals với progress
SELECT 
    sg.Id,
    sg.Name,
    sg.Description,
    sg.TargetAmount,
    sg.CurrentAmount,
    (sg.TargetAmount - sg.CurrentAmount) as RemainingAmount,
    CASE 
        WHEN sg.TargetAmount > 0 THEN ROUND((sg.CurrentAmount / sg.TargetAmount) * 100, 2)
        ELSE 0
    END as PercentageCompleted,
    DATEDIFF(day, GETDATE(), sg.TargetDate) as DaysRemaining,
    sg.Status,
    sg.ColorCode
FROM SavingGoals sg
WHERE sg.UserId = 1 AND sg.Status = 'active'
ORDER BY PercentageCompleted DESC;

-- =============================================
-- 4. TRANSACTION ANALYSIS QUERIES
-- =============================================

-- Transaction summary by month
SELECT 
    YEAR(TransactionDate) as Year,
    MONTH(TransactionDate) as Month,
    DATENAME(month, TransactionDate) as MonthName,
    SUM(CASE WHEN Type = 'income' THEN Amount ELSE 0 END) as Income,
    SUM(CASE WHEN Type = 'expense' THEN Amount ELSE 0 END) as Expenses,
    (SUM(CASE WHEN Type = 'income' THEN Amount ELSE 0 END) - 
     SUM(CASE WHEN Type = 'expense' THEN Amount ELSE 0 END)) as NetAmount
FROM Transactions
WHERE UserId = 1
GROUP BY YEAR(TransactionDate), MONTH(TransactionDate), DATENAME(month, TransactionDate)
ORDER BY Year DESC, Month DESC;

-- Top spending categories this month
SELECT 
    Category,
    COUNT(*) as TransactionCount,
    SUM(Amount) as TotalAmount,
    AVG(Amount) as AverageAmount,
    ROUND((SUM(Amount) * 100.0 / (
        SELECT SUM(Amount) 
        FROM Transactions 
        WHERE UserId = 1 
            AND Type = 'expense' 
            AND MONTH(TransactionDate) = MONTH(GETDATE())
            AND YEAR(TransactionDate) = YEAR(GETDATE())
    )), 2) as Percentage
FROM Transactions
WHERE UserId = 1 
    AND Type = 'expense'
    AND MONTH(TransactionDate) = MONTH(GETDATE())
    AND YEAR(TransactionDate) = YEAR(GETDATE())
GROUP BY Category
ORDER BY TotalAmount DESC;

-- Recent transactions with category colors
SELECT TOP 10
    t.Id,
    t.Type,
    t.Category,
    t.Amount,
    t.Description,
    t.TransactionDate,
    ISNULL(c.ColorCode, '#666666') as CategoryColor,
    ISNULL(c.IconName, 'default') as CategoryIcon
FROM Transactions t
LEFT JOIN Categories c ON t.Category = c.Name
WHERE t.UserId = 1
ORDER BY t.TransactionDate DESC, t.CreatedAt DESC;

-- Daily spending trend (last 30 days)
SELECT 
    CAST(TransactionDate as DATE) as Date,
    SUM(CASE WHEN Type = 'income' THEN Amount ELSE 0 END) as DailyIncome,
    SUM(CASE WHEN Type = 'expense' THEN Amount ELSE 0 END) as DailyExpenses
FROM Transactions
WHERE UserId = 1 
    AND TransactionDate >= DATEADD(day, -30, GETDATE())
GROUP BY CAST(TransactionDate as DATE)
ORDER BY Date DESC;

-- =============================================
-- 5. DASHBOARD QUERIES
-- =============================================

-- Complete dashboard data
WITH MonthlyStats AS (
    SELECT 
        SUM(CASE WHEN Type = 'income' THEN Amount ELSE 0 END) as MonthlyIncome,
        SUM(CASE WHEN Type = 'expense' THEN Amount ELSE 0 END) as MonthlyExpenses
    FROM Transactions
    WHERE UserId = 1 
        AND MONTH(TransactionDate) = MONTH(GETDATE())
        AND YEAR(TransactionDate) = YEAR(GETDATE())
),
BudgetStats AS (
    SELECT 
        SUM(BudgetAmount) as TotalBudget,
        SUM(SpentAmount) as TotalSpent
    FROM Budgets
    WHERE UserId = 1 
        AND Month = MONTH(GETDATE())
        AND Year = YEAR(GETDATE())
),
SavingStats AS (
    SELECT 
        SUM(TargetAmount) as TotalSavingTarget,
        SUM(CurrentAmount) as TotalSavingCurrent
    FROM SavingGoals
    WHERE UserId = 1 AND Status = 'active'
)
SELECT 
    u.FullName,
    u.AvailableBalance,
    ms.MonthlyIncome,
    ms.MonthlyExpenses,
    (ms.MonthlyIncome - ms.MonthlyExpenses) as MonthlyNet,
    bs.TotalBudget,
    bs.TotalSpent,
    CASE 
        WHEN ss.TotalSavingTarget > 0 THEN ROUND((ss.TotalSavingCurrent / ss.TotalSavingTarget) * 100, 2)
        ELSE 0
    END as SavingProgress
FROM Users u
CROSS JOIN MonthlyStats ms
CROSS JOIN BudgetStats bs
CROSS JOIN SavingStats ss
WHERE u.Id = 1;

-- =============================================
-- 6. MAINTENANCE QUERIES
-- =============================================

-- Update budget spent amounts based on actual transactions
UPDATE b
SET SpentAmount = ISNULL(t.ActualSpent, 0),
    UpdatedAt = GETUTCDATE()
FROM Budgets b
LEFT JOIN (
    SELECT 
        CategoryId,
        SUM(Amount) as ActualSpent
    FROM Transactions t2
    INNER JOIN Categories c ON t2.Category = c.Name
    WHERE t2.UserId = 1 
        AND t2.Type = 'expense'
        AND MONTH(t2.TransactionDate) = MONTH(GETDATE())
        AND YEAR(t2.TransactionDate) = YEAR(GETDATE())
    GROUP BY CategoryId
) t ON b.CategoryId = t.CategoryId
WHERE b.UserId = 1 
    AND b.Month = MONTH(GETDATE())
    AND b.Year = YEAR(GETDATE());

-- Check data integrity
SELECT 'Users' as TableName, COUNT(*) as RecordCount FROM Users
UNION ALL
SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL
SELECT 'Budgets', COUNT(*) FROM Budgets
UNION ALL
SELECT 'SavingGoals', COUNT(*) FROM SavingGoals
UNION ALL
SELECT 'Transactions', COUNT(*) FROM Transactions
UNION ALL
SELECT 'Contacts', COUNT(*) FROM Contacts
UNION ALL
SELECT 'Transfers', COUNT(*) FROM Transfers;

PRINT 'All queries executed successfully!';