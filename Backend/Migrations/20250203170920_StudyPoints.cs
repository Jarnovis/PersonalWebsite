using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class StudyPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GradeNumber",
                table: "Subject");

            migrationBuilder.RenameColumn(
                name: "Info",
                table: "Subject",
                newName: "Semester");

            migrationBuilder.RenameColumn(
                name: "GradeChar",
                table: "Subject",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Subject",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "Subject");

            migrationBuilder.RenameColumn(
                name: "Semester",
                table: "Subject",
                newName: "Info");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Subject",
                newName: "GradeChar");

            migrationBuilder.AddColumn<double>(
                name: "GradeNumber",
                table: "Subject",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
