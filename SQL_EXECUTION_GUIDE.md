# 🔧 RAILWAY POSTGRESQL - SQL EXECUTION GUIDE

## Method 1: Railway Web Interface
1. Railway Dashboard → PostgreSQL service → "Data" tab
2. Look for "Query", "SQL Editor", or "Execute SQL" option
3. Paste SQL schema and execute

## Method 2: External Database Tool

### Option A: DBeaver (Recommended - Free & Easy)
```
Download: https://dbeaver.io/download/
1. Install DBeaver Community Edition
2. New Database Connection → PostgreSQL
3. Enter Railway connection details:
   - Host: [from Railway Connect tab]
   - Port: [from Railway Connect tab]  
   - Database: railway
   - Username: postgres
   - Password: [from Railway Connect tab]
4. Test connection
5. Execute SQL script from postgresql-schema.sql
```

### Option B: pgAdmin 4
```
Download: https://www.pgadmin.org/download/
1. Install pgAdmin 4
2. Add new server with Railway details
3. Execute SQL script
```

### Option C: Online SQL Editor
```
Use: https://sqliteonline.com/
Change to PostgreSQL mode
Connect with Railway credentials
Execute schema
```

## Method 3: Terminal/Command Line
```bash
# Install PostgreSQL client
# Windows: Download from postgresql.org
# Mac: brew install postgresql

# Connect to Railway database
psql "postgresql://postgres:[password]@[host]:[port]/railway"

# Execute schema file
\i postgresql-schema.sql
```

## Expected Result After Execution:
```
✅ CREATE TABLE "Users"
✅ CREATE TABLE "Categories" 
✅ CREATE TABLE "Transactions"
✅ CREATE TABLE "Budgets"
✅ CREATE TABLE "SavingGoals"
✅ INSERT 8 sample categories
```

## Next Steps:
1. Verify tables created in Railway Data tab
2. Test backend health endpoint
3. Test mobile app registration
4. Success! 🎉