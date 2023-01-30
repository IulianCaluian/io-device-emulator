using System.Xml.Linq;
using System;

namespace ioDeviceEmulator.Client.ViewModels
{
    public class BarrierViewModel
    {
    

        private readonly object _lock = new object();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private int _rotation = 0;

        public event EventHandler<EventArgs>? RotationChanged;

        public int Rotation
        {
            get { return _rotation; }
            set
            {
                lock (_lock)
                {
                    if (value >= 0 && value <= 90)
                    {
                        _rotation = value;
                    }
                }
            }
        }

        public IList<DigitalInputViewModel> BarrierTerminals { get; private set; }

        public BarrierViewModel()
        {
            BarrierTerminals = new List<DigitalInputViewModel>();
            BarrierTerminals.Add(new DigitalInputViewModel()
            {
                Index = 0,
                Name = "Open",
                Activated = false
            });
            BarrierTerminals.Add(new DigitalInputViewModel()
            {
                Index = 1,
                Name = "Close",
                Activated = false
            });

            BarrierTerminals.Add(new DigitalInputViewModel()
            {
                Index = 2,
                Name = "Push to open button",
                Activated = false
            });
            BarrierTerminals.Add(new DigitalInputViewModel()
            {
                Index = 3,
                Name = " Photo Cell",
                Activated = false
            });
            BarrierTerminals.Add(new DigitalInputViewModel()
            {
                Index = 4,
                Name = "Loop Detector",
                Activated = false
            });

        }

        private  async Task OpenBarrier()
        {
            CancellationTokenSource cts;
            lock (_lock)
            {
                _cts.Cancel();
                _cts = cts = new CancellationTokenSource();
            }

            while (Rotation <= 90)
            {
                if (cts.IsCancellationRequested)
                    break;

                Rotation++;
                await Task.Delay(5, cts.Token);
                OnRotationChanged();
            }

        }

        private async Task CloseBarrier()
        {
            CancellationTokenSource cts;
            lock (_lock)
            {
                _cts.Cancel();
                _cts = cts = new CancellationTokenSource();
            }

            while (Rotation >= 0)
            {
                if (cts.IsCancellationRequested)
                    break;

                Rotation--;
                await Task.Delay(5, cts.Token);
                OnRotationChanged();
            }

        }

        private Task PauseBarrier()
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }

        private void OnRotationChanged()
        {
            RotationChanged?.Invoke(this, EventArgs.Empty);
        }


        public void UpdateBarrierTerminal(int terminalIndex, bool activated)
        {
            var diVM = BarrierTerminals.Where(di => di.Index == terminalIndex).FirstOrDefault();

            if (diVM == null)
                return;

            diVM.Activated = activated;
        }
    }
}
