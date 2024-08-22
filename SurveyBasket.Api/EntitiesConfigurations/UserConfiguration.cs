
namespace SurveyBasket.Api.EntitiesConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {

        builder.
            OwnsMany(x => x.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");

        builder.Property(p => p.FirstName).HasMaxLength(100);
        builder.Property(p => p.LastName).HasMaxLength(100);


    }
}
