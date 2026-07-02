using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitCore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class relationgymservice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_MemberProfiles_MemberID",
                table: "Memberships");

            migrationBuilder.RenameColumn(
                name: "MemberID",
                table: "Memberships",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Memberships_MemberID",
                table: "Memberships",
                newName: "IX_Memberships_UserId");

            migrationBuilder.AddColumn<int>(
                name: "GymServiceId",
                table: "Memberships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MemberProfileUserID",
                table: "Memberships",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_GymServiceId",
                table: "Memberships",
                column: "GymServiceId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "IX_Memberships_GymServiceId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_MemberProfileUserID",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "GymServiceId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "MemberProfileUserID",
                table: "Memberships");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Memberships",
                newName: "MemberID");

            migrationBuilder.RenameIndex(
                name: "IX_Memberships_UserId",
                table: "Memberships",
                newName: "IX_Memberships_MemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_MemberProfiles_MemberID",
                table: "Memberships",
                column: "MemberID",
                principalTable: "MemberProfiles",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
