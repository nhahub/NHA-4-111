using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitCore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class memberprofileedit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MemberProfileId",
                table: "MemberProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateIndex(
                name: "IX_MemberProfiles_MemberProfileId",
                table: "MemberProfiles",
                column: "MemberProfileId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberProfiles_MemberProfileId",
                table: "MemberProfiles");

            migrationBuilder.DropColumn(
                name: "MemberProfileId",
                table: "MemberProfiles");
        }
    }
}
