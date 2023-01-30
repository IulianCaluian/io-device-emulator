using System.ComponentModel;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ioDeviceEmulator.Client.Models
{
    public class Barrier
    {

        private readonly int _autoClosingTime;
        private readonly object _lock = new object();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private BarrierState _barrierState;

        // Control:
        public BarrierTerminal Start { get; private set; }

        /// <summary>
        /// Clsing happens only on state change to TRUE
        /// </summary>
        public BarrierTerminal Stop { get; private set; }

        /// <summary>
        /// Push button
        /// </summary>
        public BarrierTerminal Start2 { get; private set; }

        //// Safety:
        //public BarrierTerminal PhotoCell { get; private set; }
        //public BarrierTerminal LoopDetector { get; private set; }


        public event EventHandler<BarrierStateChangedEventArgs>? BarrierStateChanged;


        public Barrier(int autoClosingTimeInMs)
        {
            _autoClosingTime = autoClosingTimeInMs;

            Start = new BarrierTerminal();
            Start.PropertyChanged += Start_PropertyChanged;

            Stop = new BarrierTerminal();
            Stop.PropertyChanged += Stop_PropertyChanged;

            Start2 = new BarrierTerminal();

            //PhotoCell = new BarrierTerminal();
            //LoopDetector = new BarrierTerminal();
        }

        public void SetStateToOpened()
        {
            _barrierState = BarrierState.Opened;
            StartAutoClosingIfNoStartActivatedAndOpened();
        }

        public void SetStateToClosed()
        {
            _barrierState = BarrierState.Closed;
        }

        public BarrierState GetBarrierState()
        {
            return _barrierState;
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
                        StartOpening();
                    }
                    else
                    {
                        StartAutoClosingIfNoStartActivatedAndOpened();
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
                        ChangeStateToClosing();
                    }
                    else
                    {
                        // Nothing.
                    }
                }
            }
        }


        private void StartOpening()
        {
            lock (_lock)
            {
                _cts.Cancel();
            }
            ChangeStateToOpening();
        }

        private void ChangeStateToOpening()
        {
            if (_barrierState != BarrierState.Opened && _barrierState != BarrierState.Opening)
            {
                _barrierState = BarrierState.Opening;
                OnBarrierStateChanged(BarrierState.Opening);
            }
        }

        private async void StartAutoClosingIfNoStartActivatedAndOpened()
        {
            if (Start.Activated == false && 
                Start2.Activated == false &&
                _barrierState == BarrierState.Opened)
            {

                CancellationTokenSource cts;
                lock (_lock)
                {
                    _cts.Cancel();
                    _cts = cts = new CancellationTokenSource();
                }


                await Task.Delay(_autoClosingTime, cts.Token);
                if (!cts.IsCancellationRequested)
                    ChangeStateToClosing();
            }
        }

        private void ChangeStateToClosing()
        {
            if (_barrierState != BarrierState.Closed && _barrierState != BarrierState.Closing)
            {
                _barrierState = BarrierState.Closing;
                OnBarrierStateChanged(BarrierState.Closing);
            }
        }

    
        private void OnBarrierStateChanged(BarrierState newState)
        {
            BarrierStateChanged?.Invoke(this, new BarrierStateChangedEventArgs(newState));
        }

    }


}
