using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BokaInteDirekt.Migrations
{
    /// <inheritdoc />
    public partial class AddingCancelIdOnBookingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelId",
                table: "Bookings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelId",
                table: "Bookings");
        }
    }
}
