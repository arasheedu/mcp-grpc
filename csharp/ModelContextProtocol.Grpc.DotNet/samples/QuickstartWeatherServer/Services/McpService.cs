using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mcp.Weather;
using QuickstartWeatherServer.Tools;

public class WeatherServiceImpl : WeatherService.WeatherServiceBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherServiceImpl> _logger;

    public WeatherServiceImpl(HttpClient httpClient, ILogger<WeatherServiceImpl> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public override async Task<CallToolResult> CallTool(CallToolRequest request, ServerCallContext context)
    {
        string result = request.Params.Name switch
        {
            "GetAlerts" => await WeatherTools.GetAlerts(_httpClient, GetState(request.Params.Arguments)),
            "GetForecast" => await WeatherTools.GetForecast(_httpClient, GetLat(request.Params.Arguments), GetLon(request.Params.Arguments)),
            _ => throw new RpcException(new Status(StatusCode.NotFound, "Tool not found"))
        };

        return new CallToolResult
        {
            Content = { new Content { Text = new TextContent { Text = result, Type = "text" } } },
            IsError = false
        };
    }

    private string GetState(Struct args) => args.Fields["state"].StringValue;
    private double GetLat(Struct args) => args.Fields["latitude"].NumberValue;
    private double GetLon(Struct args) => args.Fields["longitude"].NumberValue;
}
