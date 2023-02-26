using ioDeviceEmulator.Server.Repo;

namespace ioDeviceEmulator.Server.BackgroundServices
{
    public class PeriodicDeviceStateChanger : BackgroundService
    {
        DeviceState _deviceState;

        public PeriodicDeviceStateChanger(DeviceState deviceState) { 
        
            _deviceState= deviceState;
        
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
