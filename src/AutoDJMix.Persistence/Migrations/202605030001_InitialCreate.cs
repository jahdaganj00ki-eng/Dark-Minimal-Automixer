using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoDJMix.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Tracks",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                SourcePath = table.Column<string>(type: "TEXT", nullable: false),
                DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                DurationMs = table.Column<long>(type: "INTEGER", nullable: false),
                ContentHashSha256 = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tracks", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Tracks_ContentHashSha256",
            table: "Tracks",
            column: "ContentHashSha256");

        migrationBuilder.CreateIndex(
            name: "IX_Tracks_SourcePath",
            table: "Tracks",
            column: "SourcePath",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Tracks");
    }
}
