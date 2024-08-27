using System;
using System.Reflection;

namespace SurveyBasket.Api.Mapping;
public static class MappingConfig 
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
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

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
    }
}