using System;
using System.Reflection;

namespace SurveyBasket.Api.Mapping;
public static class MappingConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
      //  TypeAdapterConfig<Student, StudentResponse>
      //.NewConfig()
      //.Map(dest => dest.FullName, src => $"{src.FirstName} {src.MiddleName} {src.LastName}")
      //.Map(dest => dest.Age, src => DateTime.Now.Year - src.DateOfBirth.Value.Year, srcCond => srcCond.DateOfBirth.HasValue);

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
    }
}