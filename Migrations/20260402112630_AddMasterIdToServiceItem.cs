using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TochkaKrasoty.Migrations
{
    /// <inheritdoc />
    public partial class AddMasterIdToServiceItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MasterId",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Services_MasterId",
                table: "Services",
                column: "MasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Masters_MasterId",
                table: "Services",
                column: "MasterId",
                principalTable: "Masters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Masters_MasterId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_MasterId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "MasterId",
                table: "Services");
        }
    }
}
