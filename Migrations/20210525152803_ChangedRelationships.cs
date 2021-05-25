using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace mcqbk.Migrations
{
    public partial class ChangedRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Exams_ExamId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamResult_AspNetUsers_EntityUserId",
                table: "ExamResult");

            migrationBuilder.DropIndex(
                name: "IX_ExamResult_EntityUserId",
                table: "ExamResult");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ExamId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "EntityUserId",
                table: "ExamResult",
                newName: "UserName");

            migrationBuilder.AddColumn<string>(
                name: "ParticipantId",
                table: "ExamResult",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamResult_ParticipantId",
                table: "ExamResult",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResult_AspNetUsers_ParticipantId",
                table: "ExamResult",
                column: "ParticipantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamResult_AspNetUsers_ParticipantId",
                table: "ExamResult");

            migrationBuilder.DropIndex(
                name: "IX_ExamResult_ParticipantId",
                table: "ExamResult");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "ExamResult");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "ExamResult",
                newName: "EntityUserId");

            migrationBuilder.AddColumn<Guid>(
                name: "ExamId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamResult_EntityUserId",
                table: "ExamResult",
                column: "EntityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ExamId",
                table: "AspNetUsers",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Exams_ExamId",
                table: "AspNetUsers",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResult_AspNetUsers_EntityUserId",
                table: "ExamResult",
                column: "EntityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
