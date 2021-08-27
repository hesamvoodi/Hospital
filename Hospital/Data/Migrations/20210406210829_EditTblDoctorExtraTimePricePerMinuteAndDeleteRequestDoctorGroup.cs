using Microsoft.EntityFrameworkCore.Migrations;

namespace Hospital.Data.Migrations
{
    public partial class EditTblDoctorExtraTimePricePerMinuteAndDeleteRequestDoctorGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestDoctorGroups");

            migrationBuilder.AddColumn<int>(
                name: "ExtraTimePricePerMinute",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraTimePricePerMinute",
                table: "Doctors");

            migrationBuilder.CreateTable(
                name: "RequestDoctorGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestDoctorGroups", x => x.Id);
                });
        }
    }
}
