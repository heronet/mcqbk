using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace mcqbk.Migrations
{
    public partial class ExamCollectionAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProvidedAnswer",
                table: "Question",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EntityUserExam",
                columns: table => new
                {
                    CreatedExamsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParticipientsId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityUserExam", x => new { x.CreatedExamsId, x.ParticipientsId });
                    table.ForeignKey(
                        name: "FK_EntityUserExam_AspNetUsers_ParticipientsId",
                        column: x => x.ParticipientsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityUserExam_Exams_CreatedExamsId",
                        column: x => x.CreatedExamsId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserExam_ParticipientsId",
                table: "EntityUserExam",
                column: "ParticipientsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityUserExam");

            migrationBuilder.DropColumn(
                name: "ProvidedAnswer",
                table: "Question");
        }
    }
}
