using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ganss.Xss;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;

namespace RelationshipAnalysis.Middlewares;

public class SanitizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HtmlSanitizer _sanitizer;

    public SanitizationMiddleware(RequestDelegate next)
    {
        _next = next;
        _sanitizer = new HtmlSanitizer();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            var type = GetRequestDtoType(context);
            if (type != null)
            {
                object sanitizedDto;
                if (type == typeof(List<string>))
                {
                    var dto = JsonConvert.DeserializeObject<IEnumerable<string>>(body);
                    sanitizedDto = SanitizeEnumerable(dto);
                }
                else
                {
                    var dto = JsonConvert.DeserializeObject(body, type);
                    sanitizedDto = SanitizeDto(dto);
                }

                var sanitizedBody = JsonConvert.SerializeObject(sanitizedDto);
                var buffer = Encoding.UTF8.GetBytes(sanitizedBody);
                context.Request.Body = new MemoryStream(buffer);
            }
        }

        await _next(context);
    }

    private Type GetRequestDtoType(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (actionDescriptor != null)
        {
            var parameters = actionDescriptor.Parameters;
            var dtoParameter =
                parameters.FirstOrDefault(p => p.ParameterType.IsClass && p.ParameterType != typeof(string));
            return dtoParameter?.ParameterType;
        }

        return null;
    }

    private IEnumerable<string> SanitizeEnumerable(IEnumerable<string> dto)
    {
        return dto.Select(str => _sanitizer.Sanitize(str));
    }

    private object SanitizeDto(object dto)
    {
        var properties = dto.GetType().GetProperties()
            .Where(p => p.PropertyType == typeof(string) && p.CanWrite && p.CanRead);

        foreach (var property in properties)
        {
            var value = (string)property.GetValue(dto);
            if (value != null) property.SetValue(dto, _sanitizer.Sanitize(value));
        }

        return dto;
    }
}