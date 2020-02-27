using Microsoft.EntityFrameworkCore.Migrations;

namespace Tokoyami.Context.Migrations
{
    public partial class Create_Word_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "hangman");

            migrationBuilder.CreateTable(
                name: "Word",
                schema: "hangman",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Word", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Word",
                schema: "hangman");
        }
    }
}
