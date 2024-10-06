namespace SurveyBasket.Api.Abstraction.Consts;

public static class DefaultRoles
{
    public partial class Admin
    {
        public const string Name = nameof(Admin);
        public const string Id = "B56D430C-0897-4B6D-B611-3AD5D43C8E55";
        public const string ConcurrencyStamp = "512CC2D6-144B-4B14-B288-F8D6788A2FF7";
    }

    public partial class Member
    {
        public const string Name = nameof(Member);
        public const string Id = "E77C8086-5256-430B-894D-0A4C4D84DA4C";
        public const string ConcurrencyStamp = "00B0718E-4A0A-4CC5-8E9B-CED22D02A022";
    }
   

}   