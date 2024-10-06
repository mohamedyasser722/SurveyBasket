using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "79CF1F77-B459-4FB4-9244-DF21652FB7C3",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFCmbjKjuPAVhgkeIMfP6E2EQ92ko4AvzKNWXgQYfJ70N4wpe2vIBUh+brUc6NPN4g==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "79CF1F77-B459-4FB4-9244-DF21652FB7C3",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEBOn6fJxrgXKXSmaKZZlBAb6lwU1BUWAQHDYneIah1MHb1NHnit/moRXgpobkNKEhQ==");
        }
    }
}
