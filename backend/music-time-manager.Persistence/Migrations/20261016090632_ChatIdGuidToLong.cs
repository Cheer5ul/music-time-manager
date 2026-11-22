using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace music_time_manager.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChatIdGuidToLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ChatId",
                table: "TelegramLinks",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ChatId",
                table: "TelegramLinks",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
