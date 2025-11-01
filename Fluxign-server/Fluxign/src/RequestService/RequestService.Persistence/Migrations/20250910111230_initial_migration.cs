using System;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "SigningRequests",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequesterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    IsSequentialSigning = table.Column<bool>(type: "boolean", nullable: false),
                    TotalRecipients = table.Column<int>(type: "integer", nullable: false),
                    CompletedSigners = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SigningRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SigningRecipients",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientName = table.Column<string>(type: "text", nullable: false),
                    RecipientPhone = table.Column<string>(type: "text", nullable: false),
                    RecipientEmail = table.Column<string>(type: "text", nullable: false),
                    SigningOrder = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SigningToken = table.Column<string>(type: "text", nullable: false),
                    LinkSentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Metadata = table.Column<JsonObject>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SigningRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SigningRecipients_SigningRequests_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "public",
                        principalTable: "SigningRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SignaturePositions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientId = table.Column<Guid>(type: "uuid", nullable: false),
                    XPosition = table.Column<decimal>(type: "numeric", nullable: false),
                    YPosition = table.Column<decimal>(type: "numeric", nullable: false),
                    PageNumber = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignaturePositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignaturePositions_SigningRecipients_RecipientId",
                        column: x => x.RecipientId,
                        principalSchema: "public",
                        principalTable: "SigningRecipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignaturePositions_SigningRequests_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "public",
                        principalTable: "SigningRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_signaturepositions_request_recipient",
                schema: "public",
                table: "SignaturePositions",
                columns: new[] { "RequestId", "RecipientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SignaturePositions_RecipientId",
                schema: "public",
                table: "SignaturePositions",
                column: "RecipientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SigningRecipients_RequestId",
                schema: "public",
                table: "SigningRecipients",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SigningRecipients_SigningToken",
                schema: "public",
                table: "SigningRecipients",
                column: "SigningToken",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SignaturePositions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "SigningRecipients",
                schema: "public");

            migrationBuilder.DropTable(
                name: "SigningRequests",
                schema: "public");
        }
    }
}
