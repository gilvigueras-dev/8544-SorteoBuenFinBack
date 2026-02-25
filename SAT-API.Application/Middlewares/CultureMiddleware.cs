using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SAT_API.Application.Middlewares;

public class CultureMiddleware
{
    private readonly RequestDelegate _next;
    private readonly List<string> _supportedCultures;

    public CultureMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _supportedCultures = configuration.GetSection("SupportedCultures").Get<List<string>>() ?? new List<string> { "es", "en" };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var culture = GetCultureFromRequest(context);

        if (!string.IsNullOrEmpty(culture) && _supportedCultures.Contains(culture))
        {
            var cultureInfo = new CultureInfo(culture);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        }

        await _next(context);
    }

    private string GetCultureFromRequest(HttpContext context)
    {
        // 1. Desde query parameter
        if (context.Request.Query.ContainsKey("lang"))
        {
            return context.Request.Query["lang"].ToString();
        }

        // 2. Desde header Accept-Language
        var acceptLanguage = context.Request.Headers.AcceptLanguage.ToString();
        if (!string.IsNullOrEmpty(acceptLanguage))
        {
            var languages = acceptLanguage.Split(',');
            foreach (var lang in languages)
            {
                var cleanLang = lang.Trim().Split(';')[0].Split('-')[0];
                if (_supportedCultures.Contains(cleanLang))
                {
                    return cleanLang;
                }
            }
        }

        // 3. Cultura por defecto
        return _supportedCultures.FirstOrDefault() ?? "es";
    }
}