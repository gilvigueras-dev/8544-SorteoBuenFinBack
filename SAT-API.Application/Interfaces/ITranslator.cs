namespace SAT_API.Application.Interfaces;

public interface ITranslator
    {
        string this[string key] { get; }
        string this[string key, string culture] { get; }
        string GetMessage(string key, string? culture = null);
        string GetMessage(string key, string culture, params object[] args);
    }
