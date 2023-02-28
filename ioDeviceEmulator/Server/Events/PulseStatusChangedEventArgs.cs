namespace ioDeviceEmulator.Server.Events
{
    public class PulseStatusChangedEventArgs : EventArgs
    {
        public int Index { get; set; }
        public int Status { get; set; }

        public PulseStatusChangedEventArgs(int index, int status)
        {
            Index = index;
            Status = status;
        }
    }
}
