-- Export your FinancialAppDb to .bak file
-- Run this in SQL Server Management Studio

BACKUP DATABASE FinancialAppDb 
TO DISK = 'D:\EXE201\FinancialApp-CleanArchitecture\FinancialAppDb.bak'
WITH FORMAT, 
     COMPRESSION,
     NAME = 'FinancialAppDb Backup for Railway Deployment';

-- Alternative: Script out tables and data
-- Right-click FinancialAppDb → Tasks → Generate Scripts
-- Choose "Script data only" or "Schema and data"
-- Save as init-railway.sql