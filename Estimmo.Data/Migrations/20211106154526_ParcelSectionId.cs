using Microsoft.EntityFrameworkCore.Migrations;

namespace Estimmo.Data.Migrations
{
    public partial class ParcelSectionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_parcel_section_id",
                table: "parcel");

            migrationBuilder.AddColumn<string>(
                name: "section_id",
                table: "parcel",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_parcel_section_id",
                table: "parcel",
                column: "section_id");

            migrationBuilder.AddForeignKey(
                name: "FK_parcel_section_section_id",
                table: "parcel",
                column: "section_id",
                principalTable: "section",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_parcel_section_section_id",
                table: "parcel");

            migrationBuilder.DropIndex(
                name: "IX_parcel_section_id",
                table: "parcel");

            migrationBuilder.DropColumn(
                name: "section_id",
                table: "parcel");

            migrationBuilder.AddForeignKey(
                name: "FK_parcel_section_id",
                table: "parcel",
                column: "id",
                principalTable: "section",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
