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

        public override async Task GetProtoStreamWeatherFrocast(Empty request, IServerStreamWriter<ProtoWeatherForecast> responseStream, ServerCallContext context)
        {
            for (int i = 0; i < 60; i++)
            {
                await Task.Delay(1000);

                var rng = new Random();
                var randomForcast = new ProtoWeatherForecast
                {
                    Date = DateTime.Now.AddDays(i),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                };

                await responseStream.WriteAsync(randomForcast);

            }
     
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
