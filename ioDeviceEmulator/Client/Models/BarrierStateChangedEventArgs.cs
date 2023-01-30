namespace ioDeviceEmulator.Client.Models
{
    public class BarrierStateChangedEventArgs : EventArgs {
        public BarrierState NewState { get; set; }
        public BarrierStateChangedEventArgs(BarrierState newState)
        {
            NewState = newState;
        }
    }
}
