using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinanceTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInstitutions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Institutions",
                columns: new[] { "Id", "Name", "Type" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Chase", "Bank" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "BMO", "Bank" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Wells Fargo", "Bank" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "Citibank", "Bank" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "US Bank", "Bank" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "PNC Bank", "Bank" },
                    { new Guid("10000000-0000-0000-0000-000000000007"), "Associated Bank", "Bank" },
                    { new Guid("10000000-0000-0000-0000-000000000008"), "Capital One", "Bank" },
                    { new Guid("10000000-0000-0000-0000-000000000009"), "Ally Bank", "Bank" },
                    { new Guid("10000000-0000-0000-0000-00000000000a"), "Navy Federal Credit Union", "Credit Union" },
                    { new Guid("10000000-0000-0000-0000-00000000000b"), "Chime", "Bank" },
                    { new Guid("10000000-0000-0000-0000-00000000000c"), "SoFi", "Bank" },
                    { new Guid("10000000-0000-0000-0000-00000000000d"), "Marcus by Goldman Sachs", "Bank" },
                    { new Guid("10000000-0000-0000-0000-00000000000e"), "Fidelity", "Brokerage" },
                    { new Guid("10000000-0000-0000-0000-00000000000f"), "Vanguard", "Brokerage" },
                    { new Guid("10000000-0000-0000-0000-000000000010"), "Charles Schwab", "Brokerage" },
                    { new Guid("10000000-0000-0000-0000-000000000011"), "E*TRADE", "Brokerage" },
                    { new Guid("10000000-0000-0000-0000-000000000012"), "Robinhood", "Brokerage" },
                    { new Guid("10000000-0000-0000-0000-000000000013"), "American Express", "Card Issuer" },
                    { new Guid("10000000-0000-0000-0000-000000000014"), "Discover", "Card Issuer" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-00000000000a"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-00000000000b"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-00000000000c"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-00000000000d"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-00000000000e"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-00000000000f"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "Institutions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000014"));
        }
    }
}
