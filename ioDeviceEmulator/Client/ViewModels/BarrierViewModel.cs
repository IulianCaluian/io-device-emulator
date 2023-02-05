using ioDeviceEmulator.Client.Models;
using System.Xml.Linq;
using System;
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


        public IList<DigitalInputViewModel> BarrierTerminals { get; private set; }
        public IList<RelayWiring> RelayWirings { get; private set; }

        private Models.Barrier _barrier;

        public event EventHandler<EventArgs>? RotatingOpenStarted;
        public event EventHandler<EventArgs>? RotationCloseStarted;

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

            RelayWirings = new List<RelayWiring>()
            {
                new RelayWiring() { RelayIndex = 0, BarrierTerminalIndex = 0 }
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

        public void UpdateBarrierTerminalWiredToRelay(int relayNumber, bool activated)
        {
            RelayWiring? rw = RelayWirings.Where(rw => rw.RelayIndex == relayNumber).FirstOrDefault();

            if (rw != null)
            {
                UpdateBarrierTerminal(rw.BarrierTerminalIndex, activated);
            }

        }





        public void SetViewModelStateToOpen()
        {
            Console.WriteLine("model set to open called");
            _barrier.SetStateToOpened();
        }

        public void SetViewModelStateToClose()
        {
            Console.WriteLine("model set to close called");
            _barrier.SetStateToClosed();
        }

        private void Barrier_StateChanged(object? sender, BarrierStateChangedEventArgs e)
        {
            if (e.NewState == BarrierState.Opening)
                OnRotationOpenStarted();

            else if (e.NewState == BarrierState.Closing)
                OnRotationCloseStarted();
        }

        private void OnRotationOpenStarted()
        {
            RotatingOpenStarted?.Invoke(this, EventArgs.Empty);
        }

        private void OnRotationCloseStarted()
        {
            RotationCloseStarted?.Invoke(this, EventArgs.Empty);
        }



    }


    public class RelayWiring
    {
        public int RelayIndex { get; set; }
        public int BarrierTerminalIndex { get; set; }
    }
}
