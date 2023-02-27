using ioDeviceEmulator.Server.Repo;
using MudBlazor;
using System;
using System.Diagnostics;

namespace ioDeviceEmulator.Server.BackgroundServices
{
    public class PeriodicDeviceStateChanger : BackgroundService
    {
        private delegate void ChangeOfState(int index, bool activated);

        DeviceModel _deviceModel;
        DeviceState _deviceState;

        private readonly object _pulsingTasksLock = new object();
        private Dictionary<int, PulsingTask> _pulsingTasks = new Dictionary<int, PulsingTask>();
        private bool _canReceiveCommands = false;


        public PeriodicDeviceStateChanger(DeviceModel deviceModel, DeviceState deviceState) 
        { 
            _deviceModel = deviceModel;
            _deviceState = deviceState;

            // Wire-up events:
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var relays = _deviceModel.GetRelays();

            lock (_pulsingTasksLock)
            {
                foreach (var rel in relays)
                {
                    if (rel.Mode == 1)
                    {
                        _pulsingTasks.Add(rel.Index, new PulsingTask(rel.Index, ChangeOfActivatedState, ChangeOfPulsingState));
                    }
                }
                _canReceiveCommands = true;
            }

            while(true)
            {
                Debug.WriteLine("Active monitoring the pulsing events.");
                if (stoppingToken.IsCancellationRequested) break;
                await Task.Delay(1000, stoppingToken);
            }
            lock (_pulsingTasksLock)
            {
                _canReceiveCommands = false;
                foreach (var pulsingTask in _pulsingTasks.Values)
                {
                    pulsingTask.SetPulsingState(0);
                }
            }
        }

        public void SetRelayPulsingState(int relayIndex, int state)
        {
            lock(_pulsingTasks)
            {
                if (_canReceiveCommands == false) return;

                if (_pulsingTasks.ContainsKey(relayIndex))
                {
                    _pulsingTasks[relayIndex].SetPulsingState(state);
                }
            }
        }

        private void ChangeOfActivatedState(int index, bool activated)
        {
            Debug.WriteLine($"Change of activated state index:{index} => activ:{activated}");
        }

        private void ChangeOfPulsingState(int index, bool pulsing)
        {
            Debug.WriteLine($"Change of pulsing state index:{index} => pulsing:{pulsing}");
        }


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

        private class PulsingTask
        {
            private readonly int _index;
            private readonly ChangeOfState _changeOfPulsingState;
            private readonly ChangeOfState _changeOfRelayStatus;

            private readonly object dataLock = new object();

            private CancellationTokenSource _cts;
            private int _pulseCount;
            private int _pulsingState;
           
            private int _timeOnWidthInSec;
            private int _timeOffWidthInSec;
            private int _pulseCountLimit;

            public PulsingTask(int index, ChangeOfState changeOfPulsingState, ChangeOfState changeOfRelayStatus)
            {
                _index = index;
                _changeOfPulsingState = changeOfPulsingState;
                _changeOfRelayStatus = changeOfRelayStatus;

                _cts = new CancellationTokenSource();

            }

            public void SetPulsingStateAndSettings(int pulsingState, PulsingSettings pulsingSettings)
            {
                lock (dataLock)
                {
                    _timeOnWidthInSec = pulsingSettings.PulseOnWidth;
                    _timeOffWidthInSec = pulsingSettings.PulseOffWidth;
                    _pulseCountLimit = pulsingSettings.PulseCountLimit;

                    _cts.Cancel();
                    _pulsingState = 0;
                    _pulseCount = 0;
                }

                SetPulsingState(pulsingState);
            }

            public void SetPulsingState(int pulsingState)
            {
                int onWdth = 2;
                int offWidth = 2;
                lock(dataLock)
                {
                    onWdth = _timeOnWidthInSec;
                    offWidth = _timeOffWidthInSec;

                    if (_pulsingState == 0 && pulsingState == 0)
                        return;

                    if (_pulsingState == 1 && pulsingState == 1)
                        return;
                
                }

                

                if (pulsingState == 1)
                {
                    // RUN:
                    _cts = new CancellationTokenSource();
                    var ct = _cts.Token;
                    _ = Task.Run(() => Iterate(onWdth, offWidth, ct), ct);
                    
                } 
                else if (pulsingState == 0)
                {
                    // STOP:
                    _cts.Cancel();
                }
            }


     
     

            private async Task Iterate(int onWidth, int offWidth, CancellationToken ct)
            {
                try
                {
                    while (true)
                    {
                        if (ct.IsCancellationRequested)
                            break;

                        // Move on ON:
                        _changeOfRelayStatus(_index, true);
                        await Task.Delay(onWidth * 1000, ct);

                        _changeOfRelayStatus(_index, false);
                        await Task.Delay(offWidth * 1000, ct);

                        lock (dataLock)
                        {
                            _pulseCount++;
                            if (_pulseCount >= _pulseCountLimit)
                            {
                                _pulseCount = 0;
                                break;
                            }
                        }
                    }
                }catch (TaskCanceledException) {

                    Debug.WriteLine("Iterate ended");
                }

                _pulsingState = 0;
                _changeOfRelayStatus(_index, false);
                _changeOfPulsingState(_index, false);
                
            }
        }
    }

  
}
