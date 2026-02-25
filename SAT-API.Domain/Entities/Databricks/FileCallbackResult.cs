using Microsoft.AspNetCore.Mvc;

namespace SAT_API.Domain.Entities.Databricks
{ 

public class FileCallbackResult : FileResult
{
    private readonly Func<Stream, CancellationToken, Task> _callback;

    public FileCallbackResult(string contentType, Func<Stream, CancellationToken, Task> callback)
        : base(contentType)
    {
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        if (context == null)
            ArgumentNullException.ThrowIfNull(context);

        var response = context.HttpContext.Response;

        response.ContentType = ContentType;

        if (!string.IsNullOrEmpty(FileDownloadName))
        {
            var headerValue = $"attachment; filename=\"{FileDownloadName}\"";
            response.Headers["Content-Disposition"] = headerValue;
        }

        if (EnableRangeProcessing)
        {
            /// Opcional: puede requerir implementación para soportar rango
            /// Aquí simplemente se activa el flag en la respuesta
            /// context.HttpContext.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpBodyControlFeature>()?.AllowSynchronousIO = true;
        }

        await _callback(response.Body, context.HttpContext.RequestAborted);
    }
}
}