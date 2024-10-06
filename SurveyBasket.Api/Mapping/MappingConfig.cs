namespace SurveyBasket.Api.Mapping;
public static class MappingConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        // Set global CascadeMode to Stop

        // for learning purposes:

        //  TypeAdapterConfig<Student, StudentResponse>
        //.NewConfig()
        //.Map(dest => dest.FullName, src => $"{src.FirstName} {src.MiddleName} {src.LastName}")
        //.Map(dest => dest.Age, src => DateTime.Now.Year - src.DateOfBirth.Value.Year, srcCond => srcCond.DateOfBirth.HasValue);

        // 1 SOLUTION) map QuestionRequest to Question by mapping List<string> in QuestionRequest class to Icollection<Answer> in Question class
        TypeAdapterConfig<QuestionRequest, Question>
           .NewConfig()
           .Map(dest => dest.Answers, src => src.Answers.Select(answer => new Answer { Content = answer }).ToList());

        // 2 SOLUTION) ignoring mapping between two properties List<string> and ICollection<Answer>
        //TypeAdapterConfig<QuestionRequest, Question>
        //    .NewConfig()
        //    .Ignore(dest => dest.Answers);


        TypeAdapterConfig<(ApplicationUser user, IList<string> roles), UserResponse>
            .NewConfig()
            .Map(dest => dest.Id, src => src.user.Id)
            .Map(dest => dest.FirstName, src => src.user.FirstName)
            .Map(dest => dest.LastName, src => src.user.LastName)
            .Map(dest => dest.Email, src => src.user.Email)
            .Map(dest => dest.IsDisabled, src => src.user.IsDisabled)
            .Map(dest => dest.Roles, src => src.roles);

        TypeAdapterConfig<CreateUserRequest, ApplicationUser>
            .NewConfig()
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.EmailConfirmed, src => true);

        TypeAdapterConfig<UpdateUserRequest, ApplicationUser>
             .NewConfig()
             .Map(dest => dest.UserName, src => src.Email);

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
    }
}