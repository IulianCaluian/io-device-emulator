using System.ComponentModel;

namespace ioDeviceEmulator.Client.Models
{
    public class BarrierTerminal : INotifyPropertyChanged
    {
        private bool _activated;
        public bool Activated { 
            get { return _activated; }
            set
            {
                _activated = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Activated"));
            }
        } 

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
