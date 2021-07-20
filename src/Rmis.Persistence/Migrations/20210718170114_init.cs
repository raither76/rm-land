using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Rmis.Persistance.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Station",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    YaCode = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Station", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackInfo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Speed = table.Column<double>(type: "double precision", nullable: true),
                    TrainNumber = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Direction",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    FromStationId = table.Column<long>(type: "bigint", nullable: true),
                    ToStationId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Direction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Direction_Station_FromStationId",
                        column: x => x.FromStationId,
                        principalTable: "Station",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Direction_Station_ToStationId",
                        column: x => x.ToStationId,
                        principalTable: "Station",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Route",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    YaId = table.Column<string>(type: "text", nullable: true),
                    DirectionId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Route", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Route_Direction_DirectionId",
                        column: x => x.DirectionId,
                        principalTable: "Direction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DepartureDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ArrivalDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RouteId = table.Column<long>(type: "bigint", nullable: true),
                    TrainDriver = table.Column<string>(type: "text", nullable: true),
                    TrainNumber = table.Column<string>(type: "text", nullable: true),
                    IsSynchronized = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedule_Route_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Route",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stop",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Duration = table.Column<double>(type: "double precision", nullable: false),
                    StationId = table.Column<long>(type: "bigint", nullable: true),
                    StopTime = table.Column<double>(type: "double precision", nullable: true),
                    RouteId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stop", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stop_Route_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Route",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stop_Station_StationId",
                        column: x => x.StationId,
                        principalTable: "Station",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Direction_FromStationId",
                table: "Direction",
                column: "FromStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Direction_ToStationId",
                table: "Direction",
                column: "ToStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Route_DirectionId",
                table: "Route",
                column: "DirectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_RouteId",
                table: "Schedule",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Stop_RouteId",
                table: "Stop",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Stop_StationId",
                table: "Stop",
                column: "StationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Stop");

            migrationBuilder.DropTable(
                name: "TrackInfo");

            migrationBuilder.DropTable(
                name: "Route");

            migrationBuilder.DropTable(
                name: "Direction");

            migrationBuilder.DropTable(
                name: "Station");
        }
    }
}
