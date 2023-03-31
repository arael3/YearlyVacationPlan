using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPU.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addStatusAndDeputyForVacationPlanModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VacationDays",
                table: "VacationPlans",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Deputy1ApplicationUserId",
                table: "VacationPlans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Deputy2ApplicationUserId",
                table: "VacationPlans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "VacationPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_VacationPlans_Deputy1ApplicationUserId",
                table: "VacationPlans",
                column: "Deputy1ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VacationPlans_Deputy2ApplicationUserId",
                table: "VacationPlans",
                column: "Deputy2ApplicationUserId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VacationPlans_AspNetUsers_Deputy1ApplicationUserId",
                table: "VacationPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_VacationPlans_AspNetUsers_Deputy2ApplicationUserId",
                table: "VacationPlans");

            migrationBuilder.DropIndex(
                name: "IX_VacationPlans_Deputy1ApplicationUserId",
                table: "VacationPlans");

            migrationBuilder.DropIndex(
                name: "IX_VacationPlans_Deputy2ApplicationUserId",
                table: "VacationPlans");

            migrationBuilder.DropColumn(
                name: "Deputy1ApplicationUserId",
                table: "VacationPlans");

            migrationBuilder.DropColumn(
                name: "Deputy2ApplicationUserId",
                table: "VacationPlans");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "VacationPlans");

            migrationBuilder.AlterColumn<string>(
                name: "VacationDays",
                table: "VacationPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
