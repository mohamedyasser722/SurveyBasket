namespace SurveyBasket.Api.EntitiesConfigurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasIndex(x => new { x.QuestionId, x.Content }).IsUnique();  // each answer should be unique in a question

        builder.Property(x => x.Content).HasMaxLength(1000);

    }
}
