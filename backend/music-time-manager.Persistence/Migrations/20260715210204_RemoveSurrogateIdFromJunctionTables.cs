using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace music_time_manager.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSurrogateIdFromJunctionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "TaskAssignees");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SubtaskAssignees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "TaskAssignees",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "SubtaskAssignees",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
