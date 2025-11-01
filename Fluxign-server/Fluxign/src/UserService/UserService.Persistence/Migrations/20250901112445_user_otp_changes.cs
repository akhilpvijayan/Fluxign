using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class user_otp_changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOtp_Users_UserId1",
                table: "UserOtp");

            migrationBuilder.DropIndex(
                name: "IX_UserOtp_UserId",
                table: "UserOtp");

            migrationBuilder.DropIndex(
                name: "IX_UserOtp_UserId1",
                table: "UserOtp");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserOtp");

            migrationBuilder.CreateIndex(
                name: "IX_UserOtp_UserId",
                table: "UserOtp",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserOtp_UserId",
                table: "UserOtp");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "UserOtp",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserOtp_UserId",
                table: "UserOtp",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserOtp_UserId1",
                table: "UserOtp",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOtp_Users_UserId1",
                table: "UserOtp",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
