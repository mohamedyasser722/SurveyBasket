namespace SurveyBasket.Api.EntitiesConfigurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        // the user should only make one vote in the same poll meaning : (PollId, UserId) should be unique
        builder.HasIndex(v => new { v.PollId, v.UserId }).IsUnique();

    }
}
