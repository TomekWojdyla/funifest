using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Funifest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocksAndDispatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedExitPlanId",
                table: "Skydivers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ManualBlocked",
                table: "Skydivers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ManualBlockedByExitPlanId",
                table: "Skydivers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignedExitPlanId",
                table: "Passengers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ManualBlocked",
                table: "Passengers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ManualBlockedByExitPlanId",
                table: "Passengers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignedExitPlanId",
                table: "Parachutes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ManualBlocked",
                table: "Parachutes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ManualBlockedByExitPlanId",
                table: "Parachutes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DispatchedAt",
                table: "ExitPlans",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ExitPlans",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedExitPlanId",
                table: "Skydivers");

            migrationBuilder.DropColumn(
                name: "ManualBlocked",
                table: "Skydivers");

            migrationBuilder.DropColumn(
                name: "ManualBlockedByExitPlanId",
                table: "Skydivers");

            migrationBuilder.DropColumn(
                name: "AssignedExitPlanId",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "ManualBlocked",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "ManualBlockedByExitPlanId",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "AssignedExitPlanId",
                table: "Parachutes");

            migrationBuilder.DropColumn(
                name: "ManualBlocked",
                table: "Parachutes");

            migrationBuilder.DropColumn(
                name: "ManualBlockedByExitPlanId",
                table: "Parachutes");

            migrationBuilder.DropColumn(
                name: "DispatchedAt",
                table: "ExitPlans");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ExitPlans");
        }
    }
}
