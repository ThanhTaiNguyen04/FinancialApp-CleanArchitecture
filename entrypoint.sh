#!/bin/bash

# Start SQL Server
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to start
sleep 30

# Run initialization script
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P FinancialApp123! -i /usr/src/app/init-db.sql

# Keep container running
wait