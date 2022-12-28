using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVillaVillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class addForeignKeyToVillaNumberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VillID",
                table: "VillaNumbers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 12, 23, 14, 26, 11, 473, DateTimeKind.Local).AddTicks(5573));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 12, 23, 14, 26, 11, 473, DateTimeKind.Local).AddTicks(5656));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 12, 23, 14, 26, 11, 473, DateTimeKind.Local).AddTicks(5663));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 12, 23, 14, 26, 11, 473, DateTimeKind.Local).AddTicks(5669));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2022, 12, 23, 14, 26, 11, 473, DateTimeKind.Local).AddTicks(5676));

            migrationBuilder.CreateIndex(
                name: "IX_VillaNumbers_VillID",
                table: "VillaNumbers",
                column: "VillID");

            migrationBuilder.AddForeignKey(
                name: "FK_VillaNumbers_Villas_VillID",
                table: "VillaNumbers",
                column: "VillID",
                principalTable: "Villas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VillaNumbers_Villas_VillID",
                table: "VillaNumbers");

            migrationBuilder.DropIndex(
                name: "IX_VillaNumbers_VillID",
                table: "VillaNumbers");

            migrationBuilder.DropColumn(
                name: "VillID",
                table: "VillaNumbers");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2022, 12, 22, 17, 50, 22, 407, DateTimeKind.Local).AddTicks(8873));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2022, 12, 22, 17, 50, 22, 407, DateTimeKind.Local).AddTicks(8971));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2022, 12, 22, 17, 50, 22, 407, DateTimeKind.Local).AddTicks(8981));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2022, 12, 22, 17, 50, 22, 407, DateTimeKind.Local).AddTicks(8987));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2022, 12, 22, 17, 50, 22, 407, DateTimeKind.Local).AddTicks(8995));
        }
    }
}
