using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyExchange.Migrations
{
    /// <inheritdoc />
    public partial class AddBankIdFieldToTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "TransferRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_BankId",
                table: "TransferRequests",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_Banks_BankId",
                table: "TransferRequests",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_Banks_BankId",
                table: "TransferRequests");

            migrationBuilder.DropIndex(
                name: "IX_TransferRequests_BankId",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "TransferRequests");
        }
    }
}
