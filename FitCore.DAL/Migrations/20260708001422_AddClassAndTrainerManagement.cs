using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitCore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddClassAndTrainerManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Classes");

            migrationBuilder.CreateTable(
                name: "ClassBooking",
                columns: table => new
                {
                    BookingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassScheduleID = table.Column<int>(type: "int", nullable: false),
                    MemberUserId = table.Column<int>(type: "int", nullable: false),
                    SessionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassBooking", x => x.BookingID);
                    table.ForeignKey(
                        name: "FK_ClassBooking_ClassSchedule_ClassScheduleID",
                        column: x => x.ClassScheduleID,
                        principalTable: "ClassSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassBooking_MemberProfiles_MemberUserId",
                        column: x => x.MemberUserId,
                        principalTable: "MemberProfiles",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrivateSession",
                columns: table => new
                {
                    PrivateSessionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainerID = table.Column<int>(type: "int", nullable: false),
                    MemberUserId = table.Column<int>(type: "int", nullable: false),
                    SessionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateSession", x => x.PrivateSessionID);
                    table.ForeignKey(
                        name: "FK_PrivateSession_MemberProfiles_MemberUserId",
                        column: x => x.MemberUserId,
                        principalTable: "MemberProfiles",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrivateSession_Trainers_TrainerID",
                        column: x => x.TrainerID,
                        principalTable: "Trainers",
                        principalColumn: "TrainerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainerWorkingHour",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainerID = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerWorkingHour", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainerWorkingHour_Trainers_TrainerID",
                        column: x => x.TrainerID,
                        principalTable: "Trainers",
                        principalColumn: "TrainerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassBooking_ClassScheduleID",
                table: "ClassBooking",
                column: "ClassScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_ClassBooking_MemberUserId",
                table: "ClassBooking",
                column: "MemberUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateSession_MemberUserId",
                table: "PrivateSession",
                column: "MemberUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateSession_TrainerID",
                table: "PrivateSession",
                column: "TrainerID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerWorkingHour_TrainerID",
                table: "TrainerWorkingHour",
                column: "TrainerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassBooking");

            migrationBuilder.DropTable(
                name: "PrivateSession");

            migrationBuilder.DropTable(
                name: "TrainerWorkingHour");

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
                table: "Classes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Classes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
