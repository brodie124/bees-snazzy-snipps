using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeesSnazzySnippets.IOptionsEnhancements;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterOptionsModel<T>(
        this IServiceCollection services,
        IConfiguration configuration
    ) where T : class, IOptionsModel {
        var sectionName = typeof(T)
            .GetProperty(nameof(IOptionsModel.SectionName), BindingFlags.Public | BindingFlags.Static)
            ?.GetValue(null)
            ?.ToString();

        if (string.IsNullOrWhiteSpace(sectionName))
            throw new InvalidOperationException($"Cannot register options model for {nameof(T)} because SectionName is null/empty");

        var section = configuration.GetSection(sectionName);
        return services.Configure<T>(section);
    }
}
