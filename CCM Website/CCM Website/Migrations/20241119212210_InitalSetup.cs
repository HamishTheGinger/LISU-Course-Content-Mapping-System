using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CCM_Website.Migrations
{
    /// <inheritdoc />
    public partial class InitalSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ActivityId);
                });

            migrationBuilder.CreateTable(
                name: "GraduateAttributes",
                columns: table => new
                {
                    AttributeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttributeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GraduateAttributes", x => x.AttributeId);
                });

            migrationBuilder.CreateTable(
                name: "LearningPlatforms",
                columns: table => new
                {
                    PlatformId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlatformName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkbookId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningPlatforms", x => x.PlatformId);
                });

            migrationBuilder.CreateTable(
                name: "LearningPlatformActivities",
                columns: table => new
                {
                    LearningPlatformId = table.Column<int>(type: "int", nullable: false),
                    ActivitiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningPlatformActivities", x => new { x.LearningPlatformId, x.ActivitiesId });
                    table.ForeignKey(
                        name: "FK_LearningPlatformActivities_Activities_ActivitiesId",
                        column: x => x.ActivitiesId,
                        principalTable: "Activities",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningPlatformActivities_LearningPlatforms_LearningPlatformId",
                        column: x => x.LearningPlatformId,
                        principalTable: "LearningPlatforms",
                        principalColumn: "PlatformId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkBooks",
                columns: table => new
                {
                    WorkbookId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseLead = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseLength = table.Column<int>(type: "int", nullable: false),
                    LearningPlatformPlatformId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkBooks", x => x.WorkbookId);
                    table.ForeignKey(
                        name: "FK_WorkBooks_LearningPlatforms_LearningPlatformPlatformId",
                        column: x => x.LearningPlatformPlatformId,
                        principalTable: "LearningPlatforms",
                        principalColumn: "PlatformId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Weeks",
                columns: table => new
                {
                    WeekId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeekNumber = table.Column<int>(type: "int", nullable: false),
                    WorkbookId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weeks", x => x.WeekId);
                    table.ForeignKey(
                        name: "FK_Weeks_WorkBooks_WorkbookId",
                        column: x => x.WorkbookId,
                        principalTable: "WorkBooks",
                        principalColumn: "WorkbookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeekActivities",
                columns: table => new
                {
                    WeekId = table.Column<int>(type: "int", nullable: false),
                    ActivitiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekActivities", x => new { x.WeekId, x.ActivitiesId });
                    table.ForeignKey(
                        name: "FK_WeekActivities_Activities_ActivitiesId",
                        column: x => x.ActivitiesId,
                        principalTable: "Activities",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeekActivities_Weeks_WeekId",
                        column: x => x.WeekId,
                        principalTable: "Weeks",
                        principalColumn: "WeekId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeekGraduateAttributes",
                columns: table => new
                {
                    WeekId = table.Column<int>(type: "int", nullable: false),
                    GraduateAttributeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekGraduateAttributes", x => new { x.WeekId, x.GraduateAttributeId });
                    table.ForeignKey(
                        name: "FK_WeekGraduateAttributes_GraduateAttributes_GraduateAttributeId",
                        column: x => x.GraduateAttributeId,
                        principalTable: "GraduateAttributes",
                        principalColumn: "AttributeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeekGraduateAttributes_Weeks_WeekId",
                        column: x => x.WeekId,
                        principalTable: "Weeks",
                        principalColumn: "WeekId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearningPlatformActivities_ActivitiesId",
                table: "LearningPlatformActivities",
                column: "ActivitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_WeekActivities_ActivitiesId",
                table: "WeekActivities",
                column: "ActivitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_WeekGraduateAttributes_GraduateAttributeId",
                table: "WeekGraduateAttributes",
                column: "GraduateAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_WorkbookId",
                table: "Weeks",
                column: "WorkbookId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkBooks_LearningPlatformPlatformId",
                table: "WorkBooks",
                column: "LearningPlatformPlatformId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearningPlatformActivities");

            migrationBuilder.DropTable(
                name: "WeekActivities");

            migrationBuilder.DropTable(
                name: "WeekGraduateAttributes");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "GraduateAttributes");

            migrationBuilder.DropTable(
                name: "Weeks");

            migrationBuilder.DropTable(
                name: "WorkBooks");

            migrationBuilder.DropTable(
                name: "LearningPlatforms");
        }
    }
}
