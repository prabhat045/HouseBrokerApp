using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseBrokerApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_Cascade_delete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyFeatures_Properties_PropertyId",
                table: "PropertyFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyImages_Properties_PropertyId",
                table: "PropertyImages");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyFeatures_Properties_PropertyId",
                table: "PropertyFeatures",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyImages_Properties_PropertyId",
                table: "PropertyImages",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyFeatures_Properties_PropertyId",
                table: "PropertyFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyImages_Properties_PropertyId",
                table: "PropertyImages");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyFeatures_Properties_PropertyId",
                table: "PropertyFeatures",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyImages_Properties_PropertyId",
                table: "PropertyImages",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id");
        }
    }
}
