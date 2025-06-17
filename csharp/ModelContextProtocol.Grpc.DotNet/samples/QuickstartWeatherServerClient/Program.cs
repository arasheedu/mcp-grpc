using Grpc.Net.Client;
using Mcp.Weather;
using static Mcp.Weather.WeatherService;

var handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback =
    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

var channel = GrpcChannel.ForAddress("https://localhost:5001",
    new GrpcChannelOptions { HttpHandler = handler });

var client = new WeatherServiceClient(channel);

var callToolRequest = new CallToolRequest
{
    Method = "mcp.weather.WeatherService",
    Params = new CallToolRequest.Types.Params
    {
        Name = "GetForecast",
        Arguments = new Google.Protobuf.WellKnownTypes.Struct
        {
            Fields =
            {
                ["latitude"] = Google.Protobuf.WellKnownTypes.Value.ForNumber(47.7601),
                ["longitude"] = Google.Protobuf.WellKnownTypes.Value.ForNumber(-122.2054)
            }
        }
    }
};

var reply = await client.CallToolAsync(callToolRequest);

Console.WriteLine("Greeting: " + reply.ToString());

Console.WriteLine("Shutting down");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
