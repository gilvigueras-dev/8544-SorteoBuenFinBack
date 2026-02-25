using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SAT_API.Application.Interfaces;

namespace SAT_API.Application.Services;

public class JsonTranslator : ITranslator
{
    private readonly IWebHostEnvironment _environment;
    private readonly Dictionary<string, Dictionary<string, string>> _translations;
    private readonly string _defaultCulture = "es";
    private readonly string[] _supportedCultures = { "es", "en", "fr" };
    public string this[string key]
    {
        get
        {
            return GetMessage(key, CurrentCulture);
        }
    }
    public string this[string key, string culture]
    {
        get
        {
            return GetMessage(key, culture);
        }
    }
    public string[] SupportedCultures => _supportedCultures;
    public string CurrentCulture
    {
        get
        {
            try
            {
                return System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            }
            catch
            {
                return _defaultCulture;
            }
        }
    }

    public JsonTranslator(IWebHostEnvironment environment)
    {
        _environment = environment;
        _translations = new Dictionary<string, Dictionary<string, string>>();
        LoadTranslations();
    }

    private void LoadTranslations()
    {
        var resourcesPath = Path.Combine(_environment.ContentRootPath, "Resources");

        if (!Directory.Exists(resourcesPath)) return;

        var languageFiles = Directory.GetFiles(resourcesPath, "language.*.json");

        foreach (var file in languageFiles)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var culture = fileName.Split('.').LastOrDefault();

                if (string.IsNullOrEmpty(culture)) continue;

                var jsonContent = File.ReadAllText(file);
                var translations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

                if (translations != null)
                {
                    _translations[culture] = translations;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error loading translation file {file}: {ex.Message}");
            }
        }
    }
    ///public string GetMessage(string key, string? culture = null)
    ///{
    ///    culture ??= _defaultCulture;

    ///    if (_translations.ContainsKey(culture) && _translations[culture].ContainsKey(key))
    ///    {
    ///        return _translations[culture][key];
    ///    }

    ///    // Fallback al idioma por defecto
    ///    if (culture != _defaultCulture && _translations.ContainsKey(_defaultCulture) &&
    ///        _translations[_defaultCulture].ContainsKey(key))
    ///    {
    ///        return _translations[_defaultCulture][key];
    ///    }

    ///    return key;
    ///}

    public string GetMessage(string key, string? culture = null)
    {
        culture ??= _defaultCulture;

        // Intenta obtener la traducción en la cultura específica
        if (_translations.TryGetValue(culture, out var translations)
            && translations.TryGetValue(key, out var message))
        {
            return message;
        }
        // Fallback al idioma por defecto
        if (culture != _defaultCulture &&
            _translations.TryGetValue(_defaultCulture, out translations) &&
            translations.TryGetValue(key, out message))
        {
            return message;
        }
                // Si no se encuentra la traducción, devuelve la clave
        return key;
    }

    public string GetMessage(string key, string culture, params object[] args)
    {
        var message = GetMessage(key, culture);
        return string.Format(message, args);
    }
}