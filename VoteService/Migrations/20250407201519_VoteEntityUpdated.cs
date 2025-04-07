using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoteSystem.Migrations
{
    /// <inheritdoc />
    public partial class VoteEntityUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                table: "Votes");

            migrationBuilder.RenameTable(
                name: "Votes",
                newName: "votes");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "votes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "votes",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "PollId",
                table: "votes",
                newName: "poll_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "votes",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ChoiceId",
                table: "votes",
                newName: "choice_id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "votes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_votes",
                table: "votes",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_votes",
                table: "votes");

            migrationBuilder.RenameTable(
                name: "votes",
                newName: "Votes");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Votes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Votes",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "poll_id",
                table: "Votes",
                newName: "PollId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Votes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "choice_id",
                table: "Votes",
                newName: "ChoiceId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Votes",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                table: "Votes",
                column: "Id");
        }
    }
}
