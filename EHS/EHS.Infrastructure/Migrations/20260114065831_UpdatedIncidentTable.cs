using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedIncidentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Incidents_IncidentNumber",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "IncidentNumber",
                table: "Incidents");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_Id",
                table: "Incidents",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Incidents_Id",
                table: "Incidents");

            migrationBuilder.AddColumn<string>(
                name: "IncidentNumber",
                table: "Incidents",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_IncidentNumber",
                table: "Incidents",
                column: "IncidentNumber",
                unique: true);
        }
    }
}
