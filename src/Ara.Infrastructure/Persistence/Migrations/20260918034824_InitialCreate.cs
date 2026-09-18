using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ara.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "destinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Island = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_destinations", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "destinations",
                columns: new[] { "Id", "Description", "ImageUrl", "Island", "Name", "Rating" },
                values: new object[,]
                {
                    { 1, "Turquoise lagoon and overwater bungalows.", null, "Society Islands", "Bora Bora", 4.9000000000000004 },
                    { 2, "Black sand beaches and the gateway to French Polynesia.", null, "Society Islands", "Tahiti", 4.5999999999999996 },
                    { 3, "Jagged volcanic peaks overlooking a coral reef.", null, "Society Islands", "Moorea", 4.7999999999999998 },
                    { 4, "One of the largest atolls in the world, prized for diving.", null, "Tuamotu Archipelago", "Rangiroa", 4.7000000000000002 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "destinations");
        }
    }
}
