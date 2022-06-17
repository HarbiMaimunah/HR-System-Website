using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class constraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var GenderValues = "ALTER TABLE Employees ADD CONSTRAINT GenderValues CHECK(Gender IN ('Female', 'Male'));";
            var JobTitles = "ALTER TABLE Employees ADD CONSTRAINT JobTitles CHECK(JobTitle IN ('Database Administrator', 'Software Developer', 'Project Manager', 'Software Engineering Director', 'HR Director', 'HR Specialist', 'CEO'));";
            var StatusValues = "ALTER TABLE VacationRequests ADD CONSTRAINT StatusValues CHECK(Status IN ('Accepted', 'Rejected', 'Pending'));";
            var VacationTypes = "ALTER TABLE VacationRequests ADD CONSTRAINT VacationTypes CHECK(VacationType IN ('Annual', 'Sick', 'Leave', 'Exceptional'));";
            var VacationDuration = "ALTER TABLE VacationRequests ADD CONSTRAINT VacationDuration CHECK(VacationDuration BETWEEN 1 AND 14);";

            migrationBuilder.Sql(GenderValues);
            migrationBuilder.Sql(JobTitles);
            migrationBuilder.Sql(StatusValues);
            migrationBuilder.Sql(VacationTypes);
            migrationBuilder.Sql(VacationDuration);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
