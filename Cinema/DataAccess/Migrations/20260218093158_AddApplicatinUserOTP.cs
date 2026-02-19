using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicatinUserOTP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "applicationUserOTPs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OTP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    ApplictionUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applicationUserOTPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_applicationUserOTPs_AspNetUsers_ApplictionUserId",
                        column: x => x.ApplictionUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_applicationUserOTPs_ApplictionUserId",
                table: "applicationUserOTPs",
                column: "ApplictionUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "applicationUserOTPs");
        }
    }
}
