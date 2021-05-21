using Microsoft.EntityFrameworkCore.Migrations;

namespace mcqbk.Migrations
{
    public partial class AddedOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasMath",
                table: "QuestionOption",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasMath",
                table: "QuestionOption");
        }
    }
}
