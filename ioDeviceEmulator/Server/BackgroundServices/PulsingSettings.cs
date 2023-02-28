namespace ioDeviceEmulator.Server.BackgroundServices
{
    public class PulsingSettings
    {
        public int PulseOnWidth { get; private set; }
        public int PulseOffWidth { get; private set; }
        public int PulseCountLimit { get; private set; }

        public PulsingSettings(int pulseOnWidth, int pulseOffWidth, int pulseCountLimit)
        {
            PulseOnWidth = pulseOnWidth;
            PulseOffWidth = pulseOffWidth;
            PulseCountLimit = pulseCountLimit;
        }
    }
}
