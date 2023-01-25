using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ioDeviceEmulator.Shared;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ioDeviceEmulator.Server.Controllers
{
    public class ProtoWeatherForecastService: 
        ioDeviceEmulator.Shared.ProtoWeatherForecastService.ProtoWeatherForecastServiceBase

    {
        private readonly Subject<string> _eventSubject;
        public Subject<string> EventSubject => _eventSubject;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public ProtoWeatherForecastService()
        {
            _eventSubject = new Subject<string>();
        }

        public override Task<ProtoWeatherForecastResponse> GetProtoWeatherForecast(Empty request, ServerCallContext context)
        {
            var response = new ProtoWeatherForecastResponse();

            response.Forecasts.AddRange(GetWeatherForecast());

            return Task.FromResult<ProtoWeatherForecastResponse>(response);
        }

        //public override async Task GetProtoStreamWeatherFrocast(Empty request, IServerStreamWriter<ProtoWeatherForecast> responseStream, ServerCallContext context)
        //{
        //    for (int i = 0; i < 60; i++)
        //    {
        //        await Task.Delay(1000);

        //        var rng = new Random();
        //        var randomForcast = new ProtoWeatherForecast
        //        {
        //            Date = DateTime.Now.AddDays(i),
        //            TemperatureC = rng.Next(-20, 55),
        //            Summary = Summaries[rng.Next(Summaries.Length)]
        //        };

        //        await responseStream.WriteAsync(randomForcast);

        //    }
        //}


        public override async Task GetProtoStreamWeatherFrocast(Empty request, IServerStreamWriter<ProtoWeatherForecast> responseStream, ServerCallContext context)
        {
            await _eventSubject
            .AsObservable()
            .ForEachAsync(async postResponseStr =>
            {
                var rng = new Random();

                var randomForcast = new ProtoWeatherForecast
                {
                    Date = DateTime.Now,
                    TemperatureC = rng.Next(-20, 55),
                    Summary = postResponseStr
                };

                try
                {
                    await responseStream.WriteAsync(randomForcast);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Some clients closed." + ex);
                }
            });
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
