using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NETCoreVSCodeDemo.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "userprofile",
                columns: table => new
                {
                    username = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userprofile", x => x.username);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userprofile");
        }
    }
}
