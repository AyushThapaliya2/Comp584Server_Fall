using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarthquakeModel.Migrations
{
    /// <inheritdoc />
    public partial class AddShelterEventRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "shelter_location_id",
                table: "earthquake_events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_earthquake_events_shelter_location_id",
                table: "earthquake_events",
                column: "shelter_location_id");

            migrationBuilder.AddForeignKey(
                name: "FK_earthquake_events_shelter_locations",
                table: "earthquake_events",
                column: "shelter_location_id",
                principalTable: "shelter_locations",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_earthquake_events_shelter_locations",
                table: "earthquake_events");

            migrationBuilder.DropIndex(
                name: "IX_earthquake_events_shelter_location_id",
                table: "earthquake_events");

            migrationBuilder.DropColumn(
                name: "shelter_location_id",
                table: "earthquake_events");
        }
    }
}
