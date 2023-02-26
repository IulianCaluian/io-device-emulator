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

            return dis.Select(di =>
            di.Mode == 0 ?
            new ProtoDigitalInput
            {
                Index = di.Index,
                Activated = ((DigitalInputDI)di).Status
            }
            :
            new ProtoDigitalInput
            {
                Index = di.Index,
                Activated = ((DigitalInputCounter)di).Activated,
            }
            ); 
        }

        public IEnumerable<ProtoRelay> GetProtoRelays()
        {
            var rs = _deviceState.GetRelays();

            return rs.Select(r =>
            r.Mode == 0 ?          
            new ProtoRelay
            {
                Index = r.Index,
                Activated = ((RelayRelay)r).Status
            }
            :
            new ProtoRelay
            {
                Index = r.Index,    
                Activated = ((RelayPulse)r).Activated
            }
            
            );
        }
    }
}
