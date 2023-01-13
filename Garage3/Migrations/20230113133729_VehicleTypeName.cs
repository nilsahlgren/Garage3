using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage3.Migrations
{
    /// <inheritdoc />
    public partial class VehicleTypeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_VehicleType_VehicleTypeId",
                table: "Vehicle");

            migrationBuilder.DropIndex(
                name: "IX_Vehicle_VehicleTypeId",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "VehicleTypeId",
                table: "Vehicle");

            migrationBuilder.AddColumn<string>(
                name: "VehicleTypeName",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleTypeName",
                table: "Vehicle");

            migrationBuilder.AddColumn<int>(
                name: "VehicleTypeId",
                table: "Vehicle",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_VehicleTypeId",
                table: "Vehicle",
                column: "VehicleTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_VehicleType_VehicleTypeId",
                table: "Vehicle",
                column: "VehicleTypeId",
                principalTable: "VehicleType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
