using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Book.Migrations.UserDb
{
    /// <inheritdoc />
    public partial class AddCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
        name: "Customers",
        columns: table => new
        {
            CustomerID = table.Column<int>(type: "int", nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
            Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Customers", x => x.CustomerID);
        });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
        name: "Customers");

        }
    }
}
