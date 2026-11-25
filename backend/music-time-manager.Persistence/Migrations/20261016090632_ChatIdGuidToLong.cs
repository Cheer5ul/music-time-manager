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
            migrationBuilder.Sql(
                """ALTER TABLE "TelegramLinks" ALTER COLUMN "ChatId" TYPE bigint USING 0::bigint;""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """ALTER TABLE "TelegramLinks" ALTER COLUMN "ChatId" TYPE uuid USING '00000000-0000-0000-0000-000000000000'::uuid;""");
        }
    }
}
