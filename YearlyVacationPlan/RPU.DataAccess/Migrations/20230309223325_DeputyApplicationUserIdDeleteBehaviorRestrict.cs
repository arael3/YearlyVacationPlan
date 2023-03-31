using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPU.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DeputyApplicationUserIdDeleteBehaviorRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VacationPlans_AspNetUsers_Deputy1ApplicationUserId",
                table: "VacationPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_VacationPlans_AspNetUsers_Deputy2ApplicationUserId",
                table: "VacationPlans");

            migrationBuilder.AddForeignKey(
                name: "FK_VacationPlans_AspNetUsers_Deputy1ApplicationUserId",
                table: "VacationPlans",
                column: "Deputy1ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VacationPlans_AspNetUsers_Deputy2ApplicationUserId",
                table: "VacationPlans",
                column: "Deputy2ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VacationPlans_AspNetUsers_Deputy1ApplicationUserId",
                table: "VacationPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_VacationPlans_AspNetUsers_Deputy2ApplicationUserId",
                table: "VacationPlans");

            migrationBuilder.AddForeignKey(
                name: "FK_VacationPlans_AspNetUsers_Deputy1ApplicationUserId",
                table: "VacationPlans",
                column: "Deputy1ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VacationPlans_AspNetUsers_Deputy2ApplicationUserId",
                table: "VacationPlans",
                column: "Deputy2ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
