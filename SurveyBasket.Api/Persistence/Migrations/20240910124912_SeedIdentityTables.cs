using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "B56D430C-0897-4B6D-B611-3AD5D43C8E55", "512CC2D6-144B-4B14-B288-F8D6788A2FF7", false, false, "Admin", "ADMIN" },
                    { "E77C8086-5256-430B-894D-0A4C4D84DA4C", "00B0718E-4A0A-4CC5-8E9B-CED22D02A022", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "79CF1F77-B459-4FB4-9244-DF21652FB7C3", 0, "9673B0D2-9CA6-4A91-82CB-5DB49F1E7376", "admin@survey-basket.com", true, "Survey Basket", "Admin", false, null, "ADMIN@SURVEY-BASKET.COM", "ADMIN@SURVEY-BASKET.COM", "AQAAAAIAAYagAAAAEFCmbjKjuPAVhgkeIMfP6E2EQ92ko4AvzKNWXgQYfJ70N4wpe2vIBUh+brUc6NPN4g==", null, false, "A54B66CD079F44D5A8818A9530D5FC78", false, "admin@survey-basket.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "Permession", "polls:read", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" },
                    { 2, "Permession", "polls:add", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" },
                    { 3, "Permession", "polls:update", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" },
                    { 4, "Permession", "polls:delete", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" },
                    { 5, "Permession", "questions:read", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" },
                    { 6, "Permession", "questions:add", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" },
                    { 7, "Permession", "questions:update", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" },
                    { 8, "Permession", "users:read", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" },
                    { 9, "Permession", "users:add", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" },
                    { 10, "Permession", "users:update", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" },
                    { 11, "Permession", "roles:read", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" },
                    { 12, "Permession", "roles:add", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" },
                    { 13, "Permession", "roles:update", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" },
                    { 14, "Permession", "results:read", "B56D430C-0897-4B6D-B611-3AD5D43C8E55" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "B56D430C-0897-4B6D-B611-3AD5D43C8E55", "79CF1F77-B459-4FB4-9244-DF21652FB7C3" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "E77C8086-5256-430B-894D-0A4C4D84DA4C");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "B56D430C-0897-4B6D-B611-3AD5D43C8E55", "79CF1F77-B459-4FB4-9244-DF21652FB7C3" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "B56D430C-0897-4B6D-B611-3AD5D43C8E55");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "79CF1F77-B459-4FB4-9244-DF21652FB7C3");
        }
    }
}
