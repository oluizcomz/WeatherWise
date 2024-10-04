using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using WeatherWise_api.Repository;
using WeatherWise_api.Services;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("https://localhost:7170")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddAuthentication().AddJwtBearer();


builder.Services.AddControllers();
builder.Services.AddSwaggerService();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.AddHttpClient<WeatherService>();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions("AWS"));
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();

builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddSingleton<IFavoriteRepository, FavoriteRepository>();


var app = builder.Build();
app.UseCors("AllowSpecificOrigin");

if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
