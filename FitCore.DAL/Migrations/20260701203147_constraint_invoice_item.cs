using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitCore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class constraint_invoice_item : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_InvoiceItem_OnlyOneTypeAllowed",
                table: "InvoiceItems",
                sql: "(ProductID IS NOT NULL AND ServiceID IS NULL) OR (ProductID IS NULL AND ServiceID IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_InvoiceItem_OnlyOneTypeAllowed",
                table: "InvoiceItems");
        }
    }
}
