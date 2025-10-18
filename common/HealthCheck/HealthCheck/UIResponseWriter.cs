using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck;

public static class UIResponseWriter
{
    //public static Task WriteHealthCheckUIResponse(HttpContext httpContext, HealthReport result)
    //{
    //    httpContext.Response.ContentType = "application/json";

    //    var json = new JObject(
    //        new JProperty("status", result.Status.ToString()),
    //        new JProperty("results", new JObject(result.Entries.Select(pair =>
    //            new JProperty(pair.Key, new JObject(
    //                new JProperty("status", pair.Value.Status.ToString()),
    //                new JProperty("description", pair.Value.Description),
    //                new JProperty("data", new JObject(pair.Value.Data.Select(
    //                    p => new JProperty(p.Key, p.Value))))))))));
    //    return httpContext.Response.WriteAsync(json.ToString(Newtonsoft.Json.Formatting.Indented));
    //}

    public static Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteStartObject("results");

            foreach (var healthReportEntry in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(healthReportEntry.Key);
                jsonWriter.WriteString("status",
                    healthReportEntry.Value.Status.ToString());
                jsonWriter.WriteString("description",
                    healthReportEntry.Value.Description);
                jsonWriter.WriteStartObject("data");

                foreach (var item in healthReportEntry.Value.Data)
                {
                    jsonWriter.WritePropertyName(item.Key);

                    JsonSerializer.Serialize(jsonWriter, item.Value,
                        item.Value?.GetType() ?? typeof(object));
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        return context.Response.WriteAsync(
            Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}
