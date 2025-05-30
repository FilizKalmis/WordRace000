using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordRace000.Migrations
{
    /// <inheritdoc />
    public partial class FixWordProgressTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordProgresses_Users_UserId",
                table: "WordProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_WordProgresses_Words_WordId",
                table: "WordProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WordProgresses",
                table: "WordProgresses");

            migrationBuilder.RenameTable(
                name: "WordProgresses",
                newName: "WordProgress");

            migrationBuilder.RenameColumn(
                name: "Turkish",
                table: "Words",
                newName: "TurkishMeaning");

            migrationBuilder.RenameColumn(
                name: "English",
                table: "Words",
                newName: "EnglishWord");

            migrationBuilder.RenameColumn(
                name: "QuizDate",
                table: "Quizzes",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_WordProgresses_WordId",
                table: "WordProgress",
                newName: "IX_WordProgress_WordId");

            migrationBuilder.RenameIndex(
                name: "IX_WordProgresses_UserId",
                table: "WordProgress",
                newName: "IX_WordProgress_UserId");

            migrationBuilder.AddColumn<string>(
                name: "AudioUrl",
                table: "Words",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "Words",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Example",
                table: "Words",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Words",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "WordProgress",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalAttempts",
                table: "WordProgress",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WordProgress",
                table: "WordProgress",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WordProgress_Users_UserId",
                table: "WordProgress",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WordProgress_Words_WordId",
                table: "WordProgress",
                column: "WordId",
                principalTable: "Words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordProgress_Users_UserId",
                table: "WordProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_WordProgress_Words_WordId",
                table: "WordProgress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WordProgress",
                table: "WordProgress");

            migrationBuilder.DropColumn(
                name: "AudioUrl",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "Example",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "WordProgress");

            migrationBuilder.DropColumn(
                name: "TotalAttempts",
                table: "WordProgress");

            migrationBuilder.RenameTable(
                name: "WordProgress",
                newName: "WordProgresses");

            migrationBuilder.RenameColumn(
                name: "TurkishMeaning",
                table: "Words",
                newName: "Turkish");

            migrationBuilder.RenameColumn(
                name: "EnglishWord",
                table: "Words",
                newName: "English");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Quizzes",
                newName: "QuizDate");

            migrationBuilder.RenameIndex(
                name: "IX_WordProgress_WordId",
                table: "WordProgresses",
                newName: "IX_WordProgresses_WordId");

            migrationBuilder.RenameIndex(
                name: "IX_WordProgress_UserId",
                table: "WordProgresses",
                newName: "IX_WordProgresses_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WordProgresses",
                table: "WordProgresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WordProgresses_Users_UserId",
                table: "WordProgresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WordProgresses_Words_WordId",
                table: "WordProgresses",
                column: "WordId",
                principalTable: "Words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
