namespace ioDeviceEmulator.Server.Repo
{
    public class DeviceState
    {
        private DeviceModel _device;
        public DeviceState()
        {
            _device = DeviceModel.E1214();
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

     

        private void SetDigitalInputDI(DigitalInput input, int status)
        {
            var di = (DigitalInputDI)input;
            di.Status = status;
        }
    }
}
