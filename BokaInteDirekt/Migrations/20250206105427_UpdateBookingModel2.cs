using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BokaInteDirekt.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingModel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Bookings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Bookings");
        }
    }
}
