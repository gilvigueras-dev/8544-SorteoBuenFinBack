using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace SAT_API.Application.Middlewares;

public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await HandleValidationException(context, ex);
            }
        }

        private static async Task HandleValidationException(HttpContext context, ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = "Errores de validación",
                errors = ex.Errors.Select(e => new
                {
                    property = e.PropertyName,
                    message = e.ErrorMessage,
                    attemptedValue = e.AttemptedValue
                })
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
