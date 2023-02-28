using Grpc.Core;
using ioDeviceEmulator.Server.BackgroundServices;
using ioDeviceEmulator.Server.Controllers;
using ioDeviceEmulator.Server.Events;
using ioDeviceEmulator.Server.GrpcServices;
using ioDeviceEmulator.Shared;
using MudBlazor;

namespace ioDeviceEmulator.Server.Repo
{
    public class DeviceState
    {
        private DeviceModel _device;
        private readonly IOEventsStreamService _ioEventsStreamService;

        public event EventHandler<PulseStatusChangedEventArgs>? RelayPuslingStatusChanged;
     
        public DeviceState(DeviceModel deviceModel, IOEventsStreamService ioEventsStreamService)
        {
            _device = deviceModel;
            _ioEventsStreamService = ioEventsStreamService;
        }

        public List<DigitalInput> GetDigitalInputs()
        {
            return _device.DigitalInputs;
        }

        public List<Relay> GetRelays()
        {
            return _device.Relays;
        }

        #region Public interface
        public bool SetInputStatus(int index, int inputStatus, string eventDescription)
        {
            DigitalInput? input = _device.DigitalInputs.Where(x => x.Index == index).FirstOrDefault();

            if (input == null)
                return false;

            if (input.Mode == 0)
            {
                var di = (DigitalInputDI)input;
                di.Status = inputStatus;
                GenerateEvent(ioElementType.DigitalInput, index, inputStatus, eventDescription);
                return true;
            } 
            else if (input.Mode == 1) 
            {
                // TODO.
            }

            return false;
        }

        public bool SetRelayStatus(int index, int status, string eventDescription)
        {
            Relay? relay = _device.Relays.Where(x => x.Index == index).FirstOrDefault();

            if (relay == null)
                return false;

            if (relay.Mode == 0)
            {
                var relRel = (RelayRelay)relay;
                relRel.Status = status;
                GenerateEvent(ioElementType.Relay, index, status, eventDescription);
                return true;
            } 
            else
            {
                var relPuls = (RelayPulse)relay;
                relPuls.PulseStatus = status;
                OnRelayPuslingStatusChanged(relPuls.Index, relPuls.PulseStatus);
                GenerateEvent(ioElementType.Relay, -1, -1, eventDescription);
                return true;
                // _periodicDeviceStateChanger.SetRelayPulsingState(relPuls.Index, relPuls.PulseStatus);
                //TODO Start pulsing + get events back on stop pulsing.
            }

            return false;
        }

        public bool UpdateRelays(IEnumerable<Relay> listRelays)
        {
            // TODO.

            // var rStatuses = string.Join("|", listRelays.Select(x => $"{x.relayIndex}:{(x.relayMode == 0 ? x.relayStatus : x.relayPulseStatus)}"));
            string relaysStatuses = string.Empty;

            var description = $"Update all relays to: {relaysStatuses}";
            GenerateEvent(ioElementType.Relay, -1, -1, description);

            throw new NotImplementedException();
        }
        #endregion






        private void OnRelayPuslingStatusChanged(int index, int status)
        {
            RelayPuslingStatusChanged?.Invoke(this, new PulseStatusChangedEventArgs(index, status));
        }

        private void GenerateEvent(ioElementType elementType, int index, int status, string description)
        {
            _ioEventsStreamService.EventSubject.OnNext(new Models.IOEvent()
            {
                EventDate = DateTime.Now,
                IOType = elementType,
                Index = index,
                Status = status,
                Summary = description
            });
        }

    }
}
