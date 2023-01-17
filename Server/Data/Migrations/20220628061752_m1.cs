using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TreasureHunt.Data.Migrations
{
    public partial class m1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Hunts",
                columns: table => new
                {
                    HuntId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hunts", x => x.HuntId);
                    table.ForeignKey(
                        name: "FK_Hunts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HuntObjects",
                columns: table => new
                {
                    HuntObjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HuntId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Coordinates = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Visible = table.Column<bool>(type: "bit", nullable: false),
                    DefaultVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuntObjects", x => x.HuntObjectId);
                    table.ForeignKey(
                        name: "FK_HuntObjects_Hunts_HuntId",
                        column: x => x.HuntId,
                        principalTable: "Hunts",
                        principalColumn: "HuntId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    ParticipantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HuntId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Accepted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.ParticipantId);
                    table.ForeignKey(
                        name: "FK_Participants_Hunts_HuntId",
                        column: x => x.HuntId,
                        principalTable: "Hunts",
                        principalColumn: "HuntId");
                    table.ForeignKey(
                        name: "FK_Participants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Locks",
                columns: table => new
                {
                    LockId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HuntObjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Locked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locks", x => x.LockId);
                    table.ForeignKey(
                        name: "FK_Locks_HuntObjects_HuntObjectId",
                        column: x => x.HuntObjectId,
                        principalTable: "HuntObjects",
                        principalColumn: "HuntObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LockId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Answered = table.Column<bool>(type: "bit", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hint = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Questions_Locks_LockId",
                        column: x => x.LockId,
                        principalTable: "Locks",
                        principalColumn: "LockId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnlockActions",
                columns: table => new
                {
                    UnlockActionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LockId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HuntObjectId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnlockActions", x => x.UnlockActionId);
                    table.ForeignKey(
                        name: "FK_UnlockActions_HuntObjects_HuntObjectId",
                        column: x => x.HuntObjectId,
                        principalTable: "HuntObjects",
                        principalColumn: "HuntObjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnlockActions_Locks_LockId",
                        column: x => x.LockId,
                        principalTable: "Locks",
                        principalColumn: "LockId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "FirstName", "LastName", "Password", "Username" },
                values: new object[] { "4584fd0c-ee67-434c-ab06-9318f5367e3a", "xxxx@example.com", "Robert", "Roe", "1234", "Snazzy101" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "FirstName", "LastName", "Password", "Username" },
                values: new object[] { "944b8464-3c10-4e45-b29a-e44ff0a727af", "xxxx@example.com", "Jakki", "Crampton", "1234", "PsychoRedHead16" });

            migrationBuilder.InsertData(
                table: "Hunts",
                columns: new[] { "HuntId", "Title", "UserId" },
                values: new object[] { "d1a9c4f0-d6bb-4c14-9867-be76b40b91e5", "Hunt for the Orange October", "944b8464-3c10-4e45-b29a-e44ff0a727af" });

            migrationBuilder.InsertData(
                table: "Hunts",
                columns: new[] { "HuntId", "Title", "UserId" },
                values: new object[] { "efd6b2e0-eabe-4ada-b3e9-edf9769a5e2d", "Hunt for the Red October", "4584fd0c-ee67-434c-ab06-9318f5367e3a" });

            migrationBuilder.InsertData(
                table: "HuntObjects",
                columns: new[] { "HuntObjectId", "Coordinates", "DefaultVisible", "HuntId", "Order", "Text", "Title", "Type", "Visible" },
                values: new object[,]
                {
                    { "6aeadcbd-5c29-4bba-b930-071de9bf45fa", "Blah", true, "efd6b2e0-eabe-4ada-b3e9-edf9769a5e2d", 0, "Here we go this is a secret place", "Secret Location 1", 0, true },
                    { "b2ba30b2-9d90-412a-9a8f-3077853d32a3", "Blah", false, "efd6b2e0-eabe-4ada-b3e9-edf9769a5e2d", 1, "Here we go this is a secret place", "Secret Location 2", 0, false }
                });

            migrationBuilder.InsertData(
                table: "Participants",
                columns: new[] { "ParticipantId", "Accepted", "HuntId", "UserId" },
                values: new object[,]
                {
                    { "38d3cf87-ef76-446f-931b-ebc6c99f338c", false, "efd6b2e0-eabe-4ada-b3e9-edf9769a5e2d", "944b8464-3c10-4e45-b29a-e44ff0a727af" },
                    { "4a01d2ca-5fdb-4f4a-b59f-d412d91d6b9d", false, "d1a9c4f0-d6bb-4c14-9867-be76b40b91e5", "4584fd0c-ee67-434c-ab06-9318f5367e3a" }
                });

            migrationBuilder.InsertData(
                table: "Locks",
                columns: new[] { "LockId", "HuntObjectId", "Locked", "Order", "Type" },
                values: new object[] { "8dc6966d-ee79-4447-84ff-63b46f8bd6f9", "6aeadcbd-5c29-4bba-b930-071de9bf45fa", true, 0, 0 });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "QuestionId", "Answer", "Answered", "Hint", "LockId", "Order", "Text", "Type" },
                values: new object[] { "e3f847d0-477c-47db-b9fc-e764d1f1c76f", "Red", false, "The answer is Red", "8dc6966d-ee79-4447-84ff-63b46f8bd6f9", 0, "What color is Red?", 0 });

            migrationBuilder.InsertData(
                table: "UnlockActions",
                columns: new[] { "UnlockActionId", "HuntObjectId", "LockId" },
                values: new object[] { "f183705c-6682-4df0-a865-d76a8f7e925e", "b2ba30b2-9d90-412a-9a8f-3077853d32a3", "8dc6966d-ee79-4447-84ff-63b46f8bd6f9" });

            migrationBuilder.CreateIndex(
                name: "IX_HuntObjects_HuntId",
                table: "HuntObjects",
                column: "HuntId");

            migrationBuilder.CreateIndex(
                name: "IX_Hunts_UserId",
                table: "Hunts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Locks_HuntObjectId",
                table: "Locks",
                column: "HuntObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_HuntId",
                table: "Participants",
                column: "HuntId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_UserId",
                table: "Participants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_LockId",
                table: "Questions",
                column: "LockId");

            migrationBuilder.CreateIndex(
                name: "IX_UnlockActions_HuntObjectId",
                table: "UnlockActions",
                column: "HuntObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UnlockActions_LockId",
                table: "UnlockActions",
                column: "LockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "UnlockActions");

            migrationBuilder.DropTable(
                name: "Locks");

            migrationBuilder.DropTable(
                name: "HuntObjects");

            migrationBuilder.DropTable(
                name: "Hunts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
