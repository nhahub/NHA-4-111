using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitCore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class membershiped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_MemberProfiles_MemberProfileId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_MemberProfileId",
                table: "Memberships");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_MemberProfileId",
                table: "Memberships",
                column: "MemberProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_MemberProfiles_MemberProfileId",
                table: "Memberships",
                column: "MemberProfileId",
                principalTable: "MemberProfiles",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_MemberProfiles_MemberProfileId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_MemberProfileId",
                table: "Memberships");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_MemberProfileId",
                table: "Memberships",
                column: "MemberProfileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_MemberProfiles_MemberProfileId",
                table: "Memberships",
                column: "MemberProfileId",
                principalTable: "MemberProfiles",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
