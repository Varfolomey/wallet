using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WalletTelegramBot.Migrations;

/// <inheritdoc />
public partial class init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        /*migrationBuilder.EnsureSchema(
            name: "public");

        migrationBuilder.CreateTable(
            name: "Incomes",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                Amount = table.Column<decimal>(type: "numeric", nullable: false),
                Comment = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Incomes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Spendings",
            schema: "public",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                Amount = table.Column<decimal>(type: "numeric", nullable: false),
                Comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Spendings", x => x.Id);
            });*/
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
