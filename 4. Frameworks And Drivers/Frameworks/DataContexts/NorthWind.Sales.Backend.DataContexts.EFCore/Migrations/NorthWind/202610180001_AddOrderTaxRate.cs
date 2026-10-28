using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using NorthWind.Sales.Backend.DataContexts.EFCore.DataContexts;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.Migrations.NorthWind;

[DbContext(typeof(NorthWindContext))]
[Migration("202610180001_AddOrderTaxRate")]
public partial class AddOrderTaxRate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) =>
        migrationBuilder.AddColumn<decimal>(
            name: "TaxRate", table: "Orders", type: "decimal(5,4)", precision: 5, scale: 4,
            nullable: false, defaultValue: 0.15m);

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropColumn(name: "TaxRate", table: "Orders");
}
