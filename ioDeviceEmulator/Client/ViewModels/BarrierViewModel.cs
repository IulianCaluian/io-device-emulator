using ioDeviceEmulator.Client.Models;
using System.Xml.Linq;
using System;
using ioDeviceEmulator.Client.Models;
using ioDeviceEmulator.Client.Pages;

namespace ioDeviceEmulator.Client.ViewModels
{
    public class BarrierViewModel
    {
        private const int _indexStart = 0;
        private const int _indexStop = 1;
        private const int _indexStart2 = 2;
        private const int _indexPhotoCell = 3;
        private const int _indexLoopDetector = 4;

        private readonly object _lock = new object();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private int _rotation = 0;
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
        public event EventHandler<EventArgs>? RotationChanged;
        
        public IList<DigitalInputViewModel> BarrierTerminals { get; private set; }

        private Models.Barrier _barrier;



        public BarrierViewModel( )
        {


            _barrier = new Models.Barrier(2000);
            _barrier.BarrierStateChanged += new EventHandler<BarrierStateChangedEventArgs>(Barrier_StateChanged);

            BarrierTerminals = new List<DigitalInputViewModel>
            {
                new DigitalInputViewModel()
                {
                    Index = _indexStart,
                    Name = "Open",
                    Activated = false
                },
                new DigitalInputViewModel()
                {
                    Index = _indexStop,
                    Name = "Close",
                    Activated = false
                },
                new DigitalInputViewModel()
                {
                    Index = _indexStart2,
                    Name = "Push to open",
                    Activated = false
                },
                new DigitalInputViewModel()
                {
                    Index = _indexPhotoCell,
                    Name = " Photo Cell",
                    Activated = false
                },
                new DigitalInputViewModel()
                {
                    Index = _indexLoopDetector,
                    Name = "Loop Detector",
                    Activated = false
                }
            };

        }

        public void UpdateBarrierTerminal(int terminalIndex, bool activated)
        {
            var diVM = BarrierTerminals.Where(di => di.Index == terminalIndex).FirstOrDefault();

            if (diVM == null)
                return;

            diVM.Activated = activated;

            if (terminalIndex == _indexStart)
            {
                _barrier.Start.Activated = activated;
            }
            else if (terminalIndex == _indexStop)
            {
                _barrier.Stop.Activated = activated;
            }
        }



        private void Barrier_StateChanged(object? sender, BarrierStateChangedEventArgs e)
        {
            if (e.NewState == BarrierState.Opening)
                Task.Run(() => OpenBarrier());

            else if (e.NewState == BarrierState.Closing)
                Task.Run(() => CloseBarrier());
        }

        private  async Task OpenBarrier()
        {
            CancellationTokenSource cts;
            lock (_lock)
            {
                _cts.Cancel();
                _cts = cts = new CancellationTokenSource();
            }

            while (Rotation < 90)
            {
                if (cts.IsCancellationRequested)
                    break;

                Rotation += 1;
                await Task.Delay(10, cts.Token);
                OnRotationChanged();
            }

            if (!cts.IsCancellationRequested)
                if (Rotation == 90)
                    _barrier.SetStateToOpened();

        }

        private async Task CloseBarrier()
        {
            CancellationTokenSource cts;
            lock (_lock)
            {
                _cts.Cancel();
                _cts = cts = new CancellationTokenSource();
            }

            while (Rotation > 0)
            {
                if (cts.IsCancellationRequested)
                    break;

                Rotation -= 1;
                await Task.Delay(10, cts.Token);
                OnRotationChanged();
            }

            if (!cts.IsCancellationRequested)
                if (Rotation == 0)
                    _barrier.SetStateToClosed();
        }

        //private Task PauseBarrier()
        //{
        //    _cts.Cancel();
        //    return Task.CompletedTask;
        //}

        private void OnRotationChanged()
        {
            RotationChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
