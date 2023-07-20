using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerApplication.Migrations
{
    /// <inheritdoc />
    public partial class BillPayStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillPayStatus",
                table: "BillPays",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CH_Transaction_Amount",
                table: "Transactions",
                sql: "Amount > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CH_BillPay_Amount",
                table: "BillPays",
                sql: "Amount > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CH_Transaction_Amount",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "CH_BillPay_Amount",
                table: "BillPays");

            migrationBuilder.DropColumn(
                name: "BillPayStatus",
                table: "BillPays");
        }
    }
}
