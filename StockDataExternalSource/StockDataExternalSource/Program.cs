using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockDataExternalSource.Models;
using StockDataExternalSource.Converters;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
                           
string accessKeyUriComponent = "access_key=" 
   + // Currently needs exported before running project
   builder.Configuration["MARKET_STACK_API_KEY"];
builder.Services.AddHttpClient("MarketStack", delegate (HttpClient httpClient)
{
    httpClient.BaseAddress = new Uri(builder.Configuration.GetConnectionString("MarketStack"));
});
WebApplication app = builder.Build();

app.Use(async delegate (HttpContext context, Func<Task> next)
{
   DateTime timestamp = DateTime.Now;
   app.Logger.LogDebug($"{timestamp}: {context.Request.Path} {context.Request.QueryString}");
   await next();
   app.Logger.LogDebug($"Response for request {context.Request.Path} {context.Request.QueryString} ({timestamp}): {context.Response.StatusCode}, {context.Response.ContentType}");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
JsonSerializerOptions inputOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};
inputOptions.Converters.Add(new CustomDateTimeOffsetConverter("yyyy-MM-ddTHH:mm:sszzz"));

app.UseHttpsRedirection();

app.MapGet("/GetEODDataByDateWindowAndSymbols", (Func<DateOnly, DateOnly, string, Task<IResult>>)async delegate (DateOnly dateFrom, DateOnly dateTo, string aggregatedSymbols)
{
    HttpResponseMessage response = await app.Services.GetService<IHttpClientFactory>().CreateClient("MarketStack").GetAsync($"/v2/eod?{accessKeyUriComponent}&date_from={dateFrom.ToString("yyyy-MM-dd")}&date_to={dateTo.ToString("yyyy-MM-dd")}&symbols={aggregatedSymbols}");
    if (response.IsSuccessStatusCode)
    {
        using (Stream contentStream = await response.Content.ReadAsStreamAsync())
        {
            EODResponse deserializedObject = await JsonSerializer.DeserializeAsync<EODResponse>(contentStream, inputOptions);
            if (deserializedObject.Data != null)
            {
                return Results.Ok(deserializedObject.Data);
            }
            return Results.NotFound("Success in querying API, but return value could not be parsed");
        }
    }
    return (response.StatusCode == HttpStatusCode.UnprocessableEntity) ? Results.NotFound("One or more stock symbols entered not found: " + aggregatedSymbols) : Results.InternalServerError();
});
app.MapGet("/ping", (Func<string>)(() => "Pong"));
app.Run();