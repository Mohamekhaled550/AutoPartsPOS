using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPartsPOS.Migrations
{
    public partial class UpdateReturnsStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Returns_PurchaseInvoices_InvoiceId",
                table: "Returns");

            migrationBuilder.DropForeignKey(
                name: "FK_Returns_SalesInvoices_InvoiceId",
                table: "Returns");

            migrationBuilder.RenameColumn(
                name: "InvoiceId",
                table: "Returns",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_Returns_InvoiceId",
                table: "Returns",
                newName: "IX_Returns_SupplierId");

            migrationBuilder.AlterColumn<int>(
                name: "ReturnType",
                table: "Returns",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Returns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseInvoiceId",
                table: "Returns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SalesInvoiceId",
                table: "Returns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReturnItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReturnItems_Returns_ReturnId",
                        column: x => x.ReturnId,
                        principalTable: "Returns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Returns_CustomerId",
                table: "Returns",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_PurchaseInvoiceId",
                table: "Returns",
                column: "PurchaseInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_SalesInvoiceId",
                table: "Returns",
                column: "SalesInvoiceId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Return_InvoiceType",
                table: "Returns",
                sql: "(ReturnType = 1 AND SalesInvoiceId IS NOT NULL AND PurchaseInvoiceId IS NULL)\r\n       OR (ReturnType = 2 AND PurchaseInvoiceId IS NOT NULL AND SalesInvoiceId IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_ProductId",
                table: "ReturnItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_ReturnId",
                table: "ReturnItems",
                column: "ReturnId");

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_Customers_CustomerId",
                table: "Returns",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_PurchaseInvoices_PurchaseInvoiceId",
                table: "Returns",
                column: "PurchaseInvoiceId",
                principalTable: "PurchaseInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_SalesInvoices_SalesInvoiceId",
                table: "Returns",
                column: "SalesInvoiceId",
                principalTable: "SalesInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_Suppliers_SupplierId",
                table: "Returns",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Returns_Customers_CustomerId",
                table: "Returns");

            migrationBuilder.DropForeignKey(
                name: "FK_Returns_PurchaseInvoices_PurchaseInvoiceId",
                table: "Returns");

            migrationBuilder.DropForeignKey(
                name: "FK_Returns_SalesInvoices_SalesInvoiceId",
                table: "Returns");

            migrationBuilder.DropForeignKey(
                name: "FK_Returns_Suppliers_SupplierId",
                table: "Returns");

            migrationBuilder.DropTable(
                name: "ReturnItems");

            migrationBuilder.DropIndex(
                name: "IX_Returns_CustomerId",
                table: "Returns");

            migrationBuilder.DropIndex(
                name: "IX_Returns_PurchaseInvoiceId",
                table: "Returns");

            migrationBuilder.DropIndex(
                name: "IX_Returns_SalesInvoiceId",
                table: "Returns");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Return_InvoiceType",
                table: "Returns");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Returns");

            migrationBuilder.DropColumn(
                name: "PurchaseInvoiceId",
                table: "Returns");

            migrationBuilder.DropColumn(
                name: "SalesInvoiceId",
                table: "Returns");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "Returns",
                newName: "InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Returns_SupplierId",
                table: "Returns",
                newName: "IX_Returns_InvoiceId");

            migrationBuilder.AlterColumn<string>(
                name: "ReturnType",
                table: "Returns",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_PurchaseInvoices_InvoiceId",
                table: "Returns",
                column: "InvoiceId",
                principalTable: "PurchaseInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_SalesInvoices_InvoiceId",
                table: "Returns",
                column: "InvoiceId",
                principalTable: "SalesInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
