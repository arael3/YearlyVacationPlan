using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPU.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DropVacationPlanIdFromTableSharedVacationPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VacationPlanId",
                table: "SharedVacationPlans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VacationPlanId",
                table: "SharedVacationPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
