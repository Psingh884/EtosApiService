using EtosApiService.Common;
using EtosApiServices.Flight.Library.Utilities;
using EtosApiServices.Model.Filter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<SwaggerExcludeSchemaFilter>();
});

builder.Services.AddHttpClient();
builder.Services.AddEtosAPIServices(builder.Configuration);

builder.Services.AddSingleton<AirlineNamesProvider>(sp =>
{
    var env = sp.GetRequiredService<IHostEnvironment>();
    var xmlPath = Path.Combine(env.ContentRootPath, "OfflineData", "AirlineNames.xml");
    return new AirlineNamesProvider(xmlPath);
});

builder.Services.AddSingleton<AirPortNamesProvider>(sp =>
{
    var env = sp.GetRequiredService<IHostEnvironment>();
    var xmlPath = Path.Combine(env.ContentRootPath, "OfflineData", "AirPortNames.xml");
    return new AirPortNamesProvider(xmlPath);
});

builder.Services.AddSingleton<AirlineCitiesProvider>(sp =>
{
    var env = sp.GetRequiredService<IHostEnvironment>();
    var xmlPath = Path.Combine(env.ContentRootPath, "OfflineData", "AirlineCities.xml");
    return new AirlineCitiesProvider(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();