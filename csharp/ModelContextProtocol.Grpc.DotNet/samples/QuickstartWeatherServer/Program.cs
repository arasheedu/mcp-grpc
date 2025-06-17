using System.Net.Http.Headers;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(5001, listenOptions =>
    {
        listenOptions.UseHttps(); // Uses development certificate by default
    });
});

builder.WebHost.UseKestrel();

builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services.AddSingleton(_ =>
{
    var client = new HttpClient() { BaseAddress = new Uri("https://api.weather.gov") };
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("weather-tool", "1.0"));
    return client;
});

// Add services
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

// Optional: Add gRPC reflection for testing
object value = builder.Services.AddGrpcReflection();

var app = builder.Build();

// Map gRPC services
app.MapGrpcService<WeatherServiceImpl>();

// Map gRPC reflection service for testing tools
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

// Add a default HTTP endpoint
app.MapGet("/", () => "MCP gRPC Server is running. Use a gRPC client to connect.");

app.Run();
