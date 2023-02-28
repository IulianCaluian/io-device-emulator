using ioDeviceEmulator.Server.Events;
using ioDeviceEmulator.Server.Repo;
using MudBlazor;
using System;
using System.Diagnostics;

namespace ioDeviceEmulator.Server.BackgroundServices
{
    public class PeriodicDeviceStateChanger : BackgroundService
    {
     

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
            _deviceState.RelayPuslingStatusChanged += new EventHandler<PulseStatusChangedEventArgs>(RelayPulseStatusChanged);
        }

        private void RelayPulseStatusChanged(object? sender, PulseStatusChangedEventArgs e)
        {
            SetRelayPulsingState(e.Index, e.Status);
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
                        var rp = (RelayPulse)rel;

                        _pulsingTasks.Add(rel.Index, new PulsingTask(rel.Index,
                            new PulsingSettings(rp.PulseOnWidth, rp.PulseOffWidth, rp.TotalCount),
                             ChangeOfPulsingState, ChangeOfActivatedState));
                    }
                }
                _canReceiveCommands = true;
            }

            foreach (var rel in relays)
            {
                if (rel.Mode == 1)
                {
                    var rp = (RelayPulse)rel;
                    SetRelayPulsingState(rel.Index, rp.PulseStatus);
                }
            }

            while (true)
            {
                // Debug.WriteLine("Active monitoring the pulsing events.");
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
            _deviceState.PulseRelayStatusActivation(index, activated);
        }

        private void ChangeOfPulsingState(int index, bool pulsing)
        {
            Debug.WriteLine($"Change of pulsing state index:{index} => pulsing:{pulsing}");
        }


     

    
    }

  
}
