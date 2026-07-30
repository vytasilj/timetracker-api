using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeHoursNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Hours",
                table: "TimeEntries",
                type: "numeric(6,2)",
                precision: 6,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(6,2)",
                oldPrecision: 6,
                oldScale: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Hours",
                table: "TimeEntries",
                type: "numeric(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(6,2)",
                oldPrecision: 6,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
