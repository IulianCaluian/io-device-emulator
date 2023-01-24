using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ioDeviceEmulator.Shared;

namespace ioDeviceEmulator.Server.Controllers
{
    public class ProtoWeatherForecastService: 
        ioDeviceEmulator.Shared.ProtoWeatherForecastService.ProtoWeatherForecastServiceBase

    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public override Task<ProtoWeatherForecastResponse> GetProtoWeatherForecast(Empty request, ServerCallContext context)
        {
            var response = new ProtoWeatherForecastResponse();

            response.Forecasts.AddRange(GetWeatherForecast());

            return Task.FromResult<ProtoWeatherForecastResponse>(response);
        }

        public IEnumerable<ProtoWeatherForecast> GetWeatherForecast()
        {
            var rng = new Random();
            return Enumerable.Range(1, 365).Select(index => new ProtoWeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }
    }
}
