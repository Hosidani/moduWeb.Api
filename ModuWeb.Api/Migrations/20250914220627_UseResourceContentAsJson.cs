using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModuWeb.Api.Migrations
{
    /// <inheritdoc />
    public partial class UseResourceContentAsJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "DefaultBookingLength",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Resources");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Resources",
                newName: "Content");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Resources",
                newName: "Type");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Resources",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DefaultBookingLength",
                table: "Resources",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Resources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Resources",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Resources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Resources",
                type: "numeric",
                nullable: true);
        }
    }
}
