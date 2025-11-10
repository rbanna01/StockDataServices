using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Persistence.Models;
using Persistence.Interfaces;
using Persistence.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Configure MongoDB connection
var dbConfig = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();
                             // Needs exported before running project
dbConfig.ConnectionString = builder.Configuration["MONGODB_CONNECTION_STRING"];
builder.Services.AddSingleton<IDatabaseSettings>(dbConfig as IDatabaseSettings);
builder.Services.AddSingleton<DTODatasetService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.UseSwaggerUI(options => 
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapGet("/DTODatasets", (Func<Task<DTODataset[]>>)async delegate
{
    DTODatasetService service = app.Services.GetService<DTODatasetService>();
    return Enumerable.ToArray(await service.GetAll());
});
app.MapGet("/DTODatasetByMetadata", (Func<string, DateTime, DateTime, Task<DTODataset>>)async delegate (string symbols, DateTime dateFrom, DateTime dateTo)
{
    DTODatasetService service = app.Services.GetService<DTODatasetService>();
    return await service.GetByMetadata(symbols.Split(",").ToList(), dateFrom, dateTo);
});
app.MapGet("/DTODatasetById", (Func<string, CancellationToken, Task<DTODataset>>)async delegate (string id, CancellationToken ct)
{
    DTODatasetService service = app.Services.GetService<DTODatasetService>();
    ObjectId filterId = new ObjectId(id);
    return await service.GetById(filterId, ct);
});
app.MapDelete("/DeleteDTODataset", (Func<string, CancellationToken, Task<IResult>>)async delegate (string id, CancellationToken ct)
{
    DTODatasetService service = app.Services.GetService<DTODatasetService>();
    return (await service.DeleteById(new ObjectId(id), ct)) ? Results.Ok(id) : Results.Problem();
});
app.MapPost("/AddDTODataset", (Func<DTODataset, CancellationToken, Task<IResult>>)async delegate (DTODataset toAdd, CancellationToken ct)
{
    DTODatasetService service = app.Services.GetService<DTODatasetService>();
    try
    {
        await service.Add(toAdd, ct);
        return Results.Created($"/DTODatasets/{toAdd.Id}", toAdd);
    }
    catch (Exception ex)
    {
        Exception e = ex;
        return Results.InternalServerError(e);
    }
});
app.MapPut("/UpdateDTODataset", (Func<string, DTODataset, CancellationToken, Task<IResult>>)async delegate (string id, DTODataset toUpdate, CancellationToken ct)
{
    DTODatasetService service = app.Services.GetService<DTODatasetService>();
    try
    {
        ObjectId newID = new ObjectId(id);
        await service.Update(newID, toUpdate, ct);
        toUpdate.Id = newID;
        return Results.Created("/DTODatasets/" + id, toUpdate);
    }
    catch (Exception ex)
    {
        Exception e = ex;
        return Results.InternalServerError(e);
    }
});
app.Run();