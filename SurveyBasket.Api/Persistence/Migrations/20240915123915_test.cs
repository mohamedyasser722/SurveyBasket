using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "79CF1F77-B459-4FB4-9244-DF21652FB7C3",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEP+IlSjS7wWHjRI01rrlTLI30SFsZu6hV54Ecs4nQkMVNVAY/J6xQHY03Q1WhfH/gg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "79CF1F77-B459-4FB4-9244-DF21652FB7C3",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELYqEk2GMLme4Qxi3/FSLVtBoHVWPub4DL169+1Yu1xKuorFIUIaXhpTdlvVE3wSBA==");
        }
    }
}
