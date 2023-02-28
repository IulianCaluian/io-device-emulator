using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ioDeviceEmulator.Server.Repo;
using ioDeviceEmulator.Shared;

namespace ioDeviceEmulator.Server.GrpcServices
{
    public class ChangeDeviceIOStatusService : Shared.ProtoChangeDeviceIOStatusService.ProtoChangeDeviceIOStatusServiceBase
    {
        private DeviceState _deviceState;
        private readonly IOEventsStreamService _ioEventsStreamService;

        public ChangeDeviceIOStatusService(DeviceState deviceState, IOEventsStreamService ioEventsStreamService)
        {
            _deviceState = deviceState;
            _ioEventsStreamService = ioEventsStreamService; 
        }

        public override Task<OperationResponse> SetProtoIOStatus(SetIOStatusRequest request, ServerCallContext context)
        {
            bool success = false;

            if (request.IoType == (int)ioElementType.DigitalInput)
            {
                success = TryToChangeStateOfDigitalInput(request.Index, request.Status);
            }
            
            var response = new OperationResponse()
            {
                Success = success
            };

            return Task.FromResult(response);
        }

        private bool TryToChangeStateOfDigitalInput(int index, int status)
        {
            if (_deviceState.SetInputStatus(index, status, status == 0 ? "Internal open digital input" : "Internal close digital input"));
            return true;  
        }
    }
}
