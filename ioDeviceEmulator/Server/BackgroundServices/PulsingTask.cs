using System.Diagnostics;

namespace ioDeviceEmulator.Server.BackgroundServices
{
    public delegate void ChangeOfState(int index, bool activated);

    public class PulsingTask
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

        public PulsingTask(int index, PulsingSettings initialPulsingSettings, ChangeOfState changeOfPulsingState, ChangeOfState changeOfRelayStatus)
        {
            _index = index;
            _changeOfPulsingState = changeOfPulsingState;
            _changeOfRelayStatus = changeOfRelayStatus;

            _timeOnWidthInSec = initialPulsingSettings.PulseOnWidth;
            _timeOffWidthInSec = initialPulsingSettings.PulseOffWidth;
            _pulseCountLimit = initialPulsingSettings.PulseCountLimit;

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

            lock (dataLock)
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
            lock (dataLock)
            {
                _pulsingState = 1;
            }

            try
            {
                while (true)
                {
                    if (ct.IsCancellationRequested)
                        break;

                    // Move on ON:
                    _changeOfRelayStatus(_index, true);
                    Debug.WriteLine($"Relay {_index} is now activated.");
                    await Task.Delay(onWidth * 1000, ct);

                    _changeOfRelayStatus(_index, false);
                    Debug.WriteLine($"Relay {_index} is now dezactivated.");
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
            }
            catch (TaskCanceledException)
            {

                Debug.WriteLine("Iterate ended");
            }

            lock (dataLock)
            {
                _pulsingState = 0;
            }

            _changeOfRelayStatus(_index, false);
            _changeOfPulsingState(_index, false);

        }
    }
}
