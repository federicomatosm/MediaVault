using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaVault.Application.Migrations
{
    /// <inheritdoc />
    public partial class addProfileImageFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProfileImages_OwnerId",
                schema: "dbo",
                table: "ProfileImages",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileImages_Customers_OwnerId",
                schema: "dbo",
                table: "ProfileImages",
                column: "OwnerId",
                principalSchema: "dbo",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileImages_Customers_OwnerId",
                schema: "dbo",
                table: "ProfileImages");

            migrationBuilder.DropIndex(
                name: "IX_ProfileImages_OwnerId",
                schema: "dbo",
                table: "ProfileImages");
        }
    }
}
