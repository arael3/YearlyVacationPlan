using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPU.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SharedVacationPlanSenderReceiverApplicationUserDeleteBehaviorRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedVacationPlans_AspNetUsers_ReceiverApplicationUserId",
                table: "SharedVacationPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedVacationPlans_AspNetUsers_SenderApplicationUserId",
                table: "SharedVacationPlans");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedVacationPlans_AspNetUsers_ReceiverApplicationUserId",
                table: "SharedVacationPlans",
                column: "ReceiverApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedVacationPlans_AspNetUsers_SenderApplicationUserId",
                table: "SharedVacationPlans",
                column: "SenderApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedVacationPlans_AspNetUsers_ReceiverApplicationUserId",
                table: "SharedVacationPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedVacationPlans_AspNetUsers_SenderApplicationUserId",
                table: "SharedVacationPlans");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedVacationPlans_AspNetUsers_ReceiverApplicationUserId",
                table: "SharedVacationPlans",
                column: "ReceiverApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedVacationPlans_AspNetUsers_SenderApplicationUserId",
                table: "SharedVacationPlans",
                column: "SenderApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
