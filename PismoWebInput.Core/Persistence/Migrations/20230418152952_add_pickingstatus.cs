using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PismoWebInput.Core.Persistence.Migrations
{
    public partial class add_pickingstatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PickingStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickingStatusID = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PickingInstructionNo = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    MFInstructionNo = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    StartPickingTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EndPickingTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    STCode = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    LineCode = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ShutterCode = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ParentItemCode = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ParentItemQty = table.Column<int>(type: "int", nullable: true),
                    ChildItemCode = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ChildItemQty = table.Column<int>(type: "int", nullable: true),
                    PickingInstructionStatus = table.Column<byte>(type: "tinyint", nullable: true),
                    MFInstructionStatus = table.Column<byte>(type: "tinyint", nullable: true),
                    User_code = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickingStatus", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickingStatus");
        }
    }
}
