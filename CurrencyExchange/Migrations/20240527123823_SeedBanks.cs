using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyExchange.Migrations
{
    /// <inheritdoc />
    public partial class SeedBanks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Banks",
                columns: new[] { "Id", "Name", "Address" },
                values: new object[,]
                {
                    { 1, "ВТБ-Банк", "просп. Ленина, 56" },
                    { 2, "ВТБ-Банк", "Красноармейская ул., 120" },
                    { 3, "ВТБ-Банк", "Театральная площадь, 4" },
                    { 4, "ВТБ-Банк", "ул. Селиванова, 64/112" },
                    { 5, "ВТБ-Банк", "Коммунистический просп., 27" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Banks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Banks",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Banks",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Banks",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Banks",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
