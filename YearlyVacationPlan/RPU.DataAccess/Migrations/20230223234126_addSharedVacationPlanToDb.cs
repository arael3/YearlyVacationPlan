using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPU.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addSharedVacationPlanToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SharedVacationPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReceiverApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SharedVacationPlanYear = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedVacationPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedVacationPlans_AspNetUsers_ReceiverApplicationUserId",
                        column: x => x.ReceiverApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SharedVacationPlans_AspNetUsers_SenderApplicationUserId",
                        column: x => x.SenderApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharedVacationPlans_ReceiverApplicationUserId",
                table: "SharedVacationPlans",
                column: "ReceiverApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedVacationPlans_SenderApplicationUserId",
                table: "SharedVacationPlans",
                column: "SenderApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharedVacationPlans");
        }
    }
}
