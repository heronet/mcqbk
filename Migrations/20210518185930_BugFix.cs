using Microsoft.EntityFrameworkCore.Migrations;

namespace mcqbk.Migrations
{
    public partial class BugFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProvidedAnswer",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "MarksObtained",
                table: "Exams");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProvidedAnswer",
                table: "Question",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MarksObtained",
                table: "Exams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
