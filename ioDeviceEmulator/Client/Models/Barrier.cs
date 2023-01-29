using System.ComponentModel;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ioDeviceEmulator.Client.Models
{
    public class Barrier
    {
        private int _degree = 0;
        private readonly int _autoClosingTime;
        private readonly int _sppedOfOneDegMoveMs;
        private readonly object _lock = new object();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public int Degree
        {
            get { return _degree; }
            set
            {
                lock (_lock)
                {
                    if (value >= 0 && value <= 90)
                    {
                        _degree = value;
                    }
                }
            }
        }


        // Control:
        public BarrierTerminal Start { get; set; }
        public BarrierTerminal Stop { get; set; }

        /// <summary>
        /// Push button
        /// </summary>
        public BarrierTerminal Start2 { get; set; } 

        // Safety:
        public BarrierTerminal PhotoCell { get; set; }
        public BarrierTerminal LoopDetector { get; set; }


        public Barrier(int autoClosingTimeInMs, int sppedOfOneDegMoveMs)
        {
            _autoClosingTime = autoClosingTimeInMs;
            _sppedOfOneDegMoveMs = sppedOfOneDegMoveMs;

            Start = new BarrierTerminal();
            Start.PropertyChanged += Start_PropertyChanged;

            Stop = new BarrierTerminal();
            Stop.PropertyChanged += Stop_PropertyChanged;
            Start2 = new BarrierTerminal();
            
            PhotoCell = new BarrierTerminal();
            LoopDetector = new BarrierTerminal();
        }

        private void Start_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Activated")
            {
                var myObject = sender as BarrierTerminal;
                if (myObject != null)
                {
                    if (myObject.Activated)
                    {
                        Task.Run(() => Open());
                    } 
                    else
                    {
                        Task.Run(() => AutomaticClose());
                    }
                    
                }
            }
        }

        private void Stop_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Activated")
            {
                var myObject = sender as BarrierTerminal;
                if (myObject != null)
                {
                    if (myObject.Activated)
                    {
                        Task.Run(() => Close());
                    }
                    else
                    {
                        // Nothing
                    }

                }
            }
        }

        // Logic:


        private async Task Open()
        {
            lock (_lock)
            {
                _cts.Cancel();
                _cts = new CancellationTokenSource();
            }

            for (int i = Degree; i <= 90; i++)
            {
                Debug.WriteLine("Here is " + i + " t:" + DateTime.UtcNow.ToString("fff"));
                Degree = i;
                await Task.Delay(_sppedOfOneDegMoveMs, _cts.Token);
                Debug.WriteLine("Here is out " + i + " t:" + DateTime.UtcNow.ToString("fff"));
            }
        }

        private async Task Close()
        {
            lock (_lock)
            {
                _cts.Cancel();
                _cts = new CancellationTokenSource();
            }

            for (int i = Degree; i >= 0; i--)
            {
                Degree = i;
                await Task.Delay(_sppedOfOneDegMoveMs, _cts.Token);
            }
        }

        private async Task AutomaticClose()
        {
            lock (_lock)
            {
                _cts.Cancel();
                _cts = new CancellationTokenSource();
            }

            await Task.Delay(_autoClosingTime, _cts.Token);
            
            await Close();
        }

    }
}
