using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Budgets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Budgets",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Budgets",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "SavingGoals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "TargetDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "SavingGoals",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "TargetDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2027, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "SavingGoals",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "TargetDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "TransactionDate" },
                values: new object[] { new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 21, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "TransactionDate" },
                values: new object[] { new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 23, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "TransactionDate" },
                values: new object[] { new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 24, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "TransactionDate" },
                values: new object[] { new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 25, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Budgets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(8517), new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(8618) });

            migrationBuilder.UpdateData(
                table: "Budgets",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(8720), new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(8721) });

            migrationBuilder.UpdateData(
                table: "Budgets",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(8724), new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(8725) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(7121));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(7227));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(7229));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(7231));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(7233));

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(4096));

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(4222));

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(4225));

            migrationBuilder.UpdateData(
                table: "SavingGoals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "TargetDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(397), new DateTime(2026, 3, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(9951), new DateTime(2025, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(496) });

            migrationBuilder.UpdateData(
                table: "SavingGoals",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "TargetDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(603), new DateTime(2027, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(593), new DateTime(2025, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(604) });

            migrationBuilder.UpdateData(
                table: "SavingGoals",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "TargetDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(609), new DateTime(2026, 1, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(607), new DateTime(2025, 9, 26, 14, 42, 12, 476, DateTimeKind.Utc).AddTicks(609) });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "TransactionDate" },
                values: new object[] { new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5604), new DateTime(2025, 9, 21, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5438) });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "TransactionDate" },
                values: new object[] { new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5712), new DateTime(2025, 9, 23, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5709) });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "TransactionDate" },
                values: new object[] { new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5715), new DateTime(2025, 9, 24, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5714) });

            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "TransactionDate" },
                values: new object[] { new DateTime(2025, 9, 26, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5717), new DateTime(2025, 9, 25, 14, 42, 12, 475, DateTimeKind.Utc).AddTicks(5717) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 26, 14, 42, 12, 474, DateTimeKind.Utc).AddTicks(7652), new DateTime(2025, 9, 26, 14, 42, 12, 474, DateTimeKind.Utc).AddTicks(7777) });
        }
    }
}
