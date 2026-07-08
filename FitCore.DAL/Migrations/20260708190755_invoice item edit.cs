using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitCore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class invoiceitemedit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_InvoiceItem_OnlyOneTypeAllowed",
                table: "InvoiceItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Memberships",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ClassID",
                table: "InvoiceItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AllowedSessionsCount",
                table: "GymService",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_ClassID",
                table: "InvoiceItems",
                column: "ClassID");

            migrationBuilder.AddCheckConstraint(
                name: "CK_InvoiceItem_TypeAllowed",
                table: "InvoiceItems",
                sql: "(CASE WHEN ProductID IS NOT NULL THEN 1 ELSE 0 END +  CASE WHEN ServiceID IS NOT NULL THEN 1 ELSE 0 END +  CASE WHEN ClassID IS NOT NULL THEN 1 ELSE 0 END) = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_Classes_ClassID",
                table: "InvoiceItems",
                column: "ClassID",
                principalTable: "Classes",
                principalColumn: "ClassID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_Classes_ClassID",
                table: "InvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_ClassID",
                table: "InvoiceItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_InvoiceItem_TypeAllowed",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "ClassID",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "AllowedSessionsCount",
                table: "GymService");

            migrationBuilder.AddCheckConstraint(
                name: "CK_InvoiceItem_OnlyOneTypeAllowed",
                table: "InvoiceItems",
                sql: "(ProductID IS NOT NULL AND ServiceID IS NULL) OR (ProductID IS NULL AND ServiceID IS NOT NULL)");
        }
    }
}
