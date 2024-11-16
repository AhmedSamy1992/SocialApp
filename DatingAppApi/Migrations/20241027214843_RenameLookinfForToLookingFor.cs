using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatingAppApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameLookinfForToLookingFor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LookinfFor",
                table: "Users",
                newName: "LookingFor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LookingFor",
                table: "Users",
                newName: "LookinfFor");
        }
    }
}
