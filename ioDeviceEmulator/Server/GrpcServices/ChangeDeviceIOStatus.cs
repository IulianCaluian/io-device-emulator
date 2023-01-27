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
            if (status == 0)
            {
                if (_deviceState.OpenInput(index))
                {
                    _ioEventsStreamService.EventSubject.OnNext(new Models.IOEvent()
                    {
                        EventDate = DateTime.Now,
                        IOType = ioElementType.DigitalInput,
                        Index = index,
                        Status = 0,
                        Summary = "Internal open digital input"
                    });

                    return true;
                }
            }
            else if (status == 1)
            {
                if (_deviceState.CloseInput(index))
                {
                    _ioEventsStreamService.EventSubject.OnNext(new Models.IOEvent()
                    {
                        EventDate = DateTime.Now,
                        IOType = ioElementType.DigitalInput,
                        Index = index,
                        Status = 1,
                        Summary = "Internal close digital input"
                    });
                }
                return true;
            }

            return false;
        }
    }
}
