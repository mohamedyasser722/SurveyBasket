namespace SurveyBasket.Api.EntitiesConfigurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasIndex(x => new { x.PollId, x.Content }).IsUnique();  //  a poll can't have the same question twice

        builder.Property(x => x.Content).HasMaxLength(1000);
    }
}
