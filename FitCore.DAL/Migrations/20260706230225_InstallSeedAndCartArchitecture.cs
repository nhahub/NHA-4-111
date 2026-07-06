using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitCore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InstallSeedAndCartArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_GymService_GymServiceId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_MemberProfiles_MemberProfileUserID",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Users_UserId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_MemberProfileUserID",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "MemberProfileUserID",
                table: "Memberships");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Memberships",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Memberships_UserId",
                table: "Memberships",
                newName: "IX_Memberships_UserID");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Trainers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Trainers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Suppliers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Suppliers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SupplierID",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Memberships",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Memberships",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Memberships",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MemberProfileId",
                table: "Memberships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "MemberProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MemberProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Invoices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Invoices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "InvoiceItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InvoiceItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "InventoryTransactionsItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InventoryTransactionsItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "InventoryTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InventoryTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Inventories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Inventories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "GymServiceClass",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "GymServiceClass",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "GymService",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "GymService",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ClassSchedule",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ClassSchedule",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Classes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Classes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CartItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CartItem",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Cart",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Cart",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AuditLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AuditLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Attendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Attendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierID",
                table: "Products",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_MemberProfileId",
                table: "Memberships",
                column: "MemberProfileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_GymService_GymServiceId",
                table: "Memberships",
                column: "GymServiceId",
                principalTable: "GymService",
                principalColumn: "ServiceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_MemberProfiles_MemberProfileId",
                table: "Memberships",
                column: "MemberProfileId",
                principalTable: "MemberProfiles",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_Users_UserID",
                table: "Memberships",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Suppliers_SupplierID",
                table: "Products",
                column: "SupplierID",
                principalTable: "Suppliers",
                principalColumn: "SupplierID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_GymService_GymServiceId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_MemberProfiles_MemberProfileId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Users_UserID",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Suppliers_SupplierID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SupplierID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_MemberProfileId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SupplierID",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "MemberProfileId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MemberProfiles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MemberProfiles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "InventoryTransactionsItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InventoryTransactionsItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "GymServiceClass");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "GymServiceClass");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "GymService");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "GymService");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ClassSchedule");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ClassSchedule");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Memberships",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Memberships_UserID",
                table: "Memberships",
                newName: "IX_Memberships_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Memberships",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MemberProfileUserID",
                table: "Memberships",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_MemberProfileUserID",
                table: "Memberships",
                column: "MemberProfileUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_GymService_GymServiceId",
                table: "Memberships",
                column: "GymServiceId",
                principalTable: "GymService",
                principalColumn: "ServiceID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_MemberProfiles_MemberProfileUserID",
                table: "Memberships",
                column: "MemberProfileUserID",
                principalTable: "MemberProfiles",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_Users_UserId",
                table: "Memberships",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
