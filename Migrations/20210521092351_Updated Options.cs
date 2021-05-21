using Microsoft.EntityFrameworkCore.Migrations;

namespace mcqbk.Migrations
{
    public partial class UpdatedOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CorrectAnswer",
                table: "Question",
                newName: "CorrectAnswerText");

            migrationBuilder.AddColumn<bool>(
                name: "CorrectAnswerHasMath",
                table: "Question",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectAnswerHasMath",
                table: "Question");

            migrationBuilder.RenameColumn(
                name: "CorrectAnswerText",
                table: "Question",
                newName: "CorrectAnswer");
        }
    }
}
