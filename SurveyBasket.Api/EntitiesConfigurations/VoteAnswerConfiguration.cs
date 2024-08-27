namespace SurveyBasket.Api.EntitiesConfigurations;

public class VoteAnswerConfiguration : IEntityTypeConfiguration<VoteAnswer>
{
    public void Configure(EntityTypeBuilder<VoteAnswer> builder)
    {
        // the same vote can't be with the same question twice meaning : (VoteId, QuestionId) should be unique

        builder.HasIndex(va => new { va.VoteId, va.QuestionId }).IsUnique();
    }
}
