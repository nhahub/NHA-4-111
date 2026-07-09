using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitCore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class membershipedit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_GymService_GymServiceId",
                table: "Memberships");

            migrationBuilder.AlterColumn<int>(
                name: "GymServiceId",
                table: "Memberships",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ClassID",
                table: "Memberships",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemainingSessions",
                table: "Memberships",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_ClassID",
                table: "Memberships",
                column: "ClassID");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_Classes_ClassID",
                table: "Memberships",
                column: "ClassID",
                principalTable: "Classes",
                principalColumn: "ClassID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_GymService_GymServiceId",
                table: "Memberships",
                column: "GymServiceId",
                principalTable: "GymService",
                principalColumn: "ServiceID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Classes_ClassID",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_GymService_GymServiceId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_ClassID",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "ClassID",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "RemainingSessions",
                table: "Memberships");

            migrationBuilder.AlterColumn<int>(
                name: "GymServiceId",
                table: "Memberships",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_GymService_GymServiceId",
                table: "Memberships",
                column: "GymServiceId",
                principalTable: "GymService",
                principalColumn: "ServiceID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
