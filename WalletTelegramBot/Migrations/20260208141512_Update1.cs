using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WalletTelegramBot.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TelegramMessageId",
                schema: "public",
                table: "Incomes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramMessageId",
                schema: "public",
                table: "Incomes");
        }
    }
}
