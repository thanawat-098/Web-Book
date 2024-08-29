using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Book.Migrations.UserDb
{
    /// <inheritdoc />
    public partial class AddRoomAndRoomRelatedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
            name: "Rooms",
            columns: table => new
            {
                RoomID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                RoomType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Rooms", x => x.RoomID);
            });

            migrationBuilder.CreateTable(
                name: "RoomImages",
                columns: table => new
                {
                    ImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomID = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomImages", x => x.ImageID);
                    table.ForeignKey(
                        name: "FK_RoomImages_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.Cascade);
                });


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
            name: "RoomImages");

            migrationBuilder.DropTable(
                name: "Rooms");

        }
    }
}
