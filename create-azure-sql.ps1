# Azure SQL Database Deployment Script
# Run this to create free Azure SQL Database

# 1. Install Azure CLI
# Download: https://aka.ms/installazurecliwindows

# 2. Login to Azure
az login

# 3. Create resource group
az group create --name FinancialAppRG --location "Southeast Asia"

# 4. Create Azure SQL Server
az sql server create \
  --name financialapp-server-2025 \
  --resource-group FinancialAppRG \
  --location "Southeast Asia" \
  --admin-user sqladmin \
  --admin-password "FinancialApp123!"

# 5. Configure firewall (allow all for testing)
az sql server firewall-rule create \
  --server financialapp-server-2025 \
  --resource-group FinancialAppRG \
  --name AllowAll \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 255.255.255.255

# 6. Create database (Free tier - 32MB)
az sql db create \
  --server financialapp-server-2025 \
  --resource-group FinancialAppRG \
  --name FinancialAppDb \
  --edition Basic \
  --capacity 5

echo "Azure SQL Database created!"
echo "Connection String:"
echo "Server=financialapp-server-2025.database.windows.net;Database=FinancialAppDb;User Id=sqladmin;Password=FinancialApp123!;Encrypt=true;TrustServerCertificate=false;"