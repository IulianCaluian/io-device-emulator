using ioDeviceEmulator.Server.BackgroundServices;
using ioDeviceEmulator.Server.Controllers;

namespace ioDeviceEmulator.Server.Repo
{
    public class DeviceState
    {
        private DeviceModel _device;

        public event EventHandler<EventArgs>? RelayPuslingStatusChanged;
     
        public DeviceState(DeviceModel deviceModel)
        {
            _device = deviceModel;
        }

        public List<DigitalInput> GetDigitalInputs()
        {
            return _device.DigitalInputs;
        }

        public List<Relay> GetRelays()
        {
            return _device.Relays;
        }

        internal bool CloseInput(int index)
        {
            DigitalInput? input = _device.DigitalInputs.Where(x => x.Index == index).FirstOrDefault();

            if (input == null)
                return false;

            if (input.Mode == 0)
            {
                SetDigitalInputDI(input, 1);
                return true;
            }

            return false;
        }

        internal bool OpenInput(int index)
        {
           DigitalInput? input =  _device.DigitalInputs.Where(x => x.Index == index).FirstOrDefault();

            if (input == null)
                return false;

            if (input.Mode == 0)
            {
                SetDigitalInputDI(input, 0);
                return true;
            }

            return false;
        }

        internal bool SetRelayStatus(int index, int status)
        {
            Relay? relay = _device.Relays.Where(x => x.Index == index).FirstOrDefault();

            if (relay == null)
                return false;

            if (relay.Mode == 0)
            {
                var relRel = (RelayRelay)relay;
                relRel.Status = status;
                return true;
            } 
            else
            {
                var relPuls = (RelayPulse)relay;
                relPuls.PulseStatus = status;
                OnRelayPuslingStatusChanged();
                // _periodicDeviceStateChanger.SetRelayPulsingState(relPuls.Index, relPuls.PulseStatus);
                //TODO Start pulsing + get events back on stop pulsing.
            }

            return false;
        }

        private void OnRelayPuslingStatusChanged()
        {
            RelayPuslingStatusChanged?.Invoke(this, new EventArgs());
        }

        public void RelayPulsingEnded(int index)
        {

        }

        internal bool UpdateRelays(IEnumerable<restRelayChannel> listRelays)
        {
            throw new NotImplementedException();
        }

        private void SetDigitalInputDI(DigitalInput input, int status)
        {
            var di = (DigitalInputDI)input;
            di.Status = status;
        }
    }
}
