using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace Altium.Dudko.Core;

public static class OptionsManager
{
    public static T Get<T>(string sectionName)
    {
        var settingsFilePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(settingsFilePath))
        {
            throw new InvalidOperationException($"Configuration file '{settingsFilePath}' not found.");
        }

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(settingsFilePath, optional: false)
            .Build();

        var options = configuration
            .GetSection(sectionName)
            .Get<T>() ?? throw new InvalidOperationException($"Missing '{sectionName}' section in appsettings.json.");

        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(options, new ValidationContext(options), results, validateAllProperties: true))
        {
            var errors = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
            throw new InvalidOperationException($"Invalid configuration:{Environment.NewLine}{errors}");
        }

        return options;
    }
}