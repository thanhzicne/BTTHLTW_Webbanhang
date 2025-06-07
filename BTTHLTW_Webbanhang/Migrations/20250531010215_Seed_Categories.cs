using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTTHLTW_Webbanhang.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Categories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "Categories",
              columns: new[] { "Name" },
              values: new object[,]
              {
                    { "Sách sinh vật" },
                    { "Sách động vật" },
                    { "Sách thực vật" }
              });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
