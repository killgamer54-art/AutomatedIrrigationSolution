using System;
using IrrigationScada.Web.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IrrigationScada.Web.Data.Migrations
{
    /// <summary>
    /// Лабораторна робота №8: створення таблиці dbo.SensorSamples для історії показань.
    /// Згенеровано командою: Add-Migration AddSensorSamples
    /// </summary>
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261020120000_AddSensorSamples")]
    public partial class AddSensorSamples : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SensorSamples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MeasuredAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorSamples", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SensorSamples");
        }
    }
}
