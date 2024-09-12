using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Api.EntitiesConfigurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        // Default Data

        var permessions = Permissions.GetAllPermessions();

        var adminClaims = new List<IdentityRoleClaim<string>>();

        for (int i = 0; i < permessions.Count; i++)
        {
            adminClaims.Add(new IdentityRoleClaim<string>
            {
                Id = i + 1,
                ClaimType = Permissions.Type,
                ClaimValue = permessions[i],
                RoleId = DefaultRoles.AdminRoleId
            });
        }

        builder.HasData(adminClaims);

    }
}
