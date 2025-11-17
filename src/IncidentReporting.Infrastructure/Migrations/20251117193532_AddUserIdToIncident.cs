using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentReporting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToIncident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Delete all existing incidents to avoid foreign key constraint issues
            // since they don't have valid UserId values
            migrationBuilder.Sql("DELETE FROM Incidents;");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Incidents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_UserId",
                table: "Incidents",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Users_UserId",
                table: "Incidents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Users_UserId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_UserId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Incidents");
        }
    }
}
