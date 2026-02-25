using Microsoft.AspNetCore.Http;
using SAT_API.Application.Interfaces;

namespace SAT_API.Application.Services;

public class AddressClientService : IAddressClientService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddressClientService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string ObtenerIPCliente()
    {
        var context = _httpContextAccessor.HttpContext;

        // Verificar headers de proxy
        var xForwardedFor = context?.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            return xForwardedFor.Split(',')[0].Trim();
        }

        return context?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
    }
}
