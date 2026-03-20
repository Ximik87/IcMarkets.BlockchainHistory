using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IcMarkets.BlockchainHistory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blockchain_snapshots",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    blockchain_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    source_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    raw_json = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    height = table.Column<long>(type: "bigint", nullable: true),
                    hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    peer_count = table.Column<int>(type: "integer", nullable: true),
                    unconfirmed_count = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blockchain_snapshots", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_blockchain_snapshots_type_created",
                table: "blockchain_snapshots",
                columns: new[] { "blockchain_type", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blockchain_snapshots");
        }
    }
}
