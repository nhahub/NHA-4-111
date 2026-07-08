using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitCore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class attendenceedit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Classes_ClassID",
                table: "Attendances");

            migrationBuilder.AddColumn<int>(
                name: "MembershipID",
                table: "Attendances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_MembershipID",
                table: "Attendances",
                column: "MembershipID");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Classes_ClassID",
                table: "Attendances",
                column: "ClassID",
                principalTable: "Classes",
                principalColumn: "ClassID");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Memberships_MembershipID",
                table: "Attendances",
                column: "MembershipID",
                principalTable: "Memberships",
                principalColumn: "MembershipID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Classes_ClassID",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Memberships_MembershipID",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_MembershipID",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "MembershipID",
                table: "Attendances");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Classes_ClassID",
                table: "Attendances",
                column: "ClassID",
                principalTable: "Classes",
                principalColumn: "ClassID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
