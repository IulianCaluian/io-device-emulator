using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ioDeviceEmulator.Server.Models;
using ioDeviceEmulator.Shared;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ioDeviceEmulator.Server.GrpcServices
{
    public class IOEventsStreamService : Shared.ProtoIOEventsStreamService.ProtoIOEventsStreamServiceBase
    {
        public Subject<IOEvent> EventSubject => _eventSubject;
        private readonly Subject<IOEvent> _eventSubject;


        public IOEventsStreamService()
        {
            _eventSubject = new Subject<IOEvent>();    
        }

        public override async Task GetProtoIOEventsStream(Empty request, IServerStreamWriter<ProtoIOEvent> responseStream, ServerCallContext context)
        {
            await _eventSubject
            .AsObservable()
            .ForEachAsync(async ioEvent =>
            {
                var protoIOEvent = new ProtoIOEvent()
                {
                    Date = ioEvent.EventDate,
                    IoType = (int)ioEvent.IOType,
                    Index = ioEvent.Index,
                    Status = ioEvent.Status,
                    Summary = ioEvent.Summary
                };

                try
                {
                    await responseStream.WriteAsync(protoIOEvent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Some clients closed." + ex);
                }
            });
        }
    }
}
