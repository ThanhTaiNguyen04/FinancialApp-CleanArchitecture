﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinancialApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesBudgetsSavingGoals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IconName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AvailableBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    BudgetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budgets_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Budgets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsRecent = table.Column<bool>(type: "bit", nullable: false),
                    LastTransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavingGoals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IconName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavingGoals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IconName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<int>(type: "int", nullable: false),
                    ToContactId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransferType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfers_Contacts_ToContactId",
                        column: x => x.ToContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_Users_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "ColorCode", "CreatedAt", "IconName", "IsActive", "Name", "Type" },
                values: new object[,]
                {
                    { 1, "#4CAF50", new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(7121), "salary", true, "Lương", "income" },
                    { 2, "#FF9800", new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(7227), "shopping", true, "Đi chợ", "expense" },
                    { 3, "#2196F3", new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(7229), "home", true, "Thuê nhà", "expense" },
                    { 4, "#F44336", new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(7231), "medical", true, "Y tế", "expense" },
                    { 5, "#9C27B0", new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(7233), "travel", true, "Du lịch", "expense" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvailableBalance", "AvatarUrl", "CreatedAt", "Email", "FullName", "Phone", "UpdatedAt" },
                values: new object[] { 1, 5320.50m, "https://images.pexels.com/photos/2167673/pexels-photo-2167673.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260", new DateTime(2025, 9, 26, 14, 42, 12, 474, DateTimeKind.Utc).AddTicks(7652), "christopher.summers@email.com", "Christopher Summers", "+1234567890", new DateTime(2025, 9, 26, 14, 42, 12, 474, DateTimeKind.Utc).AddTicks(7777) });

            migrationBuilder.InsertData(
                table: "Budgets",
                columns: new[] { "Id", "BudgetAmount", "CategoryId", "CreatedAt", "Month", "SpentAmount", "UpdatedAt", "UserId", "Year" },
                values: new object[,]
                {
                    { 1, 2000000m, 2, new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(8517), 9, 455789m, new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(8618), 1, 2025 },
                    { 2, 3500000m, 3, new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(8720), 9, 3000000m, new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(8721), 1, 2025 },
                    { 3, 1000000m, 4, new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(8724), 9, 243789m, new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(8725), 1, 2025 }
                });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "Email", "FullName", "IsRecent", "LastTransactionDate", "Phone", "UserId" },
                values: new object[,]
                {
                    { 1, "https://images.pexels.com/photos/733872/pexels-photo-733872.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260", new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(4096), "catherine@email.com", "Catherine Johnson", true, null, "+1234567891", 1 },
                    { 2, "https://images.pexels.com/photos/697509/pexels-photo-697509.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260", new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(4222), "allan@email.com", "Allan Smith", true, null, "+1234567892", 1 },
                    { 3, "https://images.pexels.com/photos/2748091/pexels-photo-2748091.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=750&w=1260", new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(4225), "kimberly@email.com", "Kimberly Brown", true, null, "+1234567893", 1 }
                });

            migrationBuilder.InsertData(
                table: "SavingGoals",
                columns: new[] { "Id", "ColorCode", "CreatedAt", "CurrentAmount", "Description", "IconName", "Name", "Status", "TargetAmount", "TargetDate", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, "#4CAF50", new DateTime(2025, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(397), 2300000m, "Tiết kiệm cho việc học", "education", "Giáo dục", "active", 5000000m, new DateTime(2026, 3, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(9951), new DateTime(2025, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(496), 1 },
                    { 2, "#FF9800", new DateTime(2025, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(603), 5600000m, "Tiết kiệm mua nhà", "house", "Mua nhà", "active", 47000000m, new DateTime(2027, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(593), new DateTime(2025, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(604), 1 },
                    { 3, "#2196F3", new DateTime(2025, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(609), 800000m, "Chuyến du lịch Hàn Quốc", "travel", "Du lịch", "active", 2200000m, new DateTime(2026, 1, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(607), new DateTime(2025, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(609), 1 }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "Category", "CategoryId", "CreatedAt", "Description", "IconName", "TransactionDate", "Type", "UserId" },
                values: new object[,]
                {
                    { 1, 16000000m, "Lương", null, new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5604), "Lương tháng 9", null, new DateTime(2025, 9, 21, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5438), "income", 1 },
                    { 2, 455789m, "Đi chợ", null, new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5712), "Mua sắm hàng tuần", null, new DateTime(2025, 9, 23, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5709), "expense", 1 },
                    { 3, 3000000m, "Thuê nhà", null, new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5715), "Tiền thuê nhà tháng 9", null, new DateTime(2025, 9, 24, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5714), "expense", 1 },
                    { 4, 243789m, "Y tế", null, new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5717), "Khám bệnh và thuốc", null, new DateTime(2025, 9, 25, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5717), "expense", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_CategoryId",
                table: "Budgets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_UserId",
                table: "Budgets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId_Phone",
                table: "Contacts",
                columns: new[] { "UserId", "Phone" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavingGoals_UserId",
                table: "SavingGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId_TransactionDate",
                table: "Transactions",
                columns: new[] { "UserId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_FromUserId_TransferDate",
                table: "Transfers",
                columns: new[] { "FromUserId", "TransferDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ToContactId",
                table: "Transfers",
                column: "ToContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropTable(
                name: "SavingGoals");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
