using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IcMarkets.BlockchainHistory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingBlockchainFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "hash",
                table: "blockchain_snapshots",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "high_fee_per_kb",
                table: "blockchain_snapshots",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_fork_hash",
                table: "blockchain_snapshots",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "last_fork_height",
                table: "blockchain_snapshots",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "latest_url",
                table: "blockchain_snapshots",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "low_fee_per_kb",
                table: "blockchain_snapshots",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "medium_fee_per_kb",
                table: "blockchain_snapshots",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "previous_hash",
                table: "blockchain_snapshots",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "previous_url",
                table: "blockchain_snapshots",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "time",
                table: "blockchain_snapshots",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_blockchain_snapshots_hash",
                table: "blockchain_snapshots",
                column: "hash",
                unique: true,
                filter: "hash IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_blockchain_snapshots_hash",
                table: "blockchain_snapshots");

            migrationBuilder.DropColumn(
                name: "high_fee_per_kb",
                table: "blockchain_snapshots");

            migrationBuilder.DropColumn(
                name: "last_fork_hash",
                table: "blockchain_snapshots");

            migrationBuilder.DropColumn(
                name: "last_fork_height",
                table: "blockchain_snapshots");

            migrationBuilder.DropColumn(
                name: "latest_url",
                table: "blockchain_snapshots");

            migrationBuilder.DropColumn(
                name: "low_fee_per_kb",
                table: "blockchain_snapshots");

            migrationBuilder.DropColumn(
                name: "medium_fee_per_kb",
                table: "blockchain_snapshots");

            migrationBuilder.DropColumn(
                name: "previous_hash",
                table: "blockchain_snapshots");

            migrationBuilder.DropColumn(
                name: "previous_url",
                table: "blockchain_snapshots");

            migrationBuilder.DropColumn(
                name: "time",
                table: "blockchain_snapshots");

            migrationBuilder.AlterColumn<string>(
                name: "hash",
                table: "blockchain_snapshots",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);
        }
    }
}
