using Microsoft.Extensions.Configuration;

namespace Altium.Dudko.Core;

public static class OptionsManager
{
    public static T Get<T>(string sectionName)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var options = configuration
            .GetSection(sectionName)
            .Get<T>() ?? throw new InvalidOperationException($"Missing '{sectionName}' section in appsettings.json.");

        return options;
    }
}