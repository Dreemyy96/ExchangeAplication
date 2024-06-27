using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyExchange.Migrations
{
    /// <inheritdoc />
    public partial class SeedBanksWithCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Applications");

            migrationBuilder.Sql("DELETE FROM TransferRequests");

            migrationBuilder.Sql("DELETE FROM Banks");

            migrationBuilder.InsertData(
                table: "Banks",
                columns: new[] { "Id", "Name", "Address" },
                values: new object[,]
                {
                    { 1, "Альфа-Банк", "просп. Космонавтов, 15" },
                    { 2, "Альфа-Банк", "площадь Гагарина, 6/87" },
                    { 3, "Альфа-Банк", "Коммунистический просп., 27" },
                    { 4, "Альфа-Банк", "1-я Майская ул., 3" },
                    { 5, "Альфа-Банк", "Ворошиловский просп., 31" }
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
