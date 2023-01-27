using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ioDeviceEmulator.Server.Repo;
using ioDeviceEmulator.Shared;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ioDeviceEmulator.Server.GrpcServices
{
    public class DeviceStateService  : Shared.ProtoDeviceStateService.ProtoDeviceStateServiceBase
    {
        DeviceState _deviceState;

        public DeviceStateService(DeviceState deviceState)
        {
            _deviceState = deviceState;
        }

        public override Task<ProtoDeviceStateResponse> GetProtoDeviceState(Empty request, ServerCallContext context)
        {
            var response = new ProtoDeviceStateResponse();

            response.DigitalInputs.AddRange(GetProtoDigitalInputs());

            response.Relays.AddRange(GetProtoRelays());

            return Task.FromResult(response);
        }

        public IEnumerable<ProtoDigitalInput> GetProtoDigitalInputs()
        {
            var dis = _deviceState.GetDigitalInputs();

            return dis.Select(di => new ProtoDigitalInput
            {
                Index = di.Index,
                Status = ((DigitalInputDI)di).Status
            }); 
        }

        public IEnumerable<ProtoRelay> GetProtoRelays()
        {
            var rs = _deviceState.GetRelays();

            return rs.Select(r => new ProtoRelay
            {
                Index = r.Index,
                Status = ((RelayRelay)r).Status
            });
        }
    }
}
