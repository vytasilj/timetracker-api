using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TimeTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectRateHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRates_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRates_ProjectId_EffectiveFrom",
                table: "ProjectRates",
                columns: new[] { "ProjectId", "EffectiveFrom" },
                unique: true);

            migrationBuilder.Sql(@"
                INSERT INTO ""ProjectRates"" (""ProjectId"", ""HourlyRate"", ""EffectiveFrom"")
                SELECT ""Id"", ""DefaultHourlyRate"", DATE '2000-01-01'
                FROM ""Projects"";
            ");

            migrationBuilder.DropColumn(
                name: "DefaultHourlyRate",
                table: "Projects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectRates");

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultHourlyRate",
                table: "Projects",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
