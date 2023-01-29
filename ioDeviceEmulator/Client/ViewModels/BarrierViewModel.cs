namespace ioDeviceEmulator.Client.ViewModels
{
    public class BarrierViewModel
    {
  
        private readonly object _lock = new object();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private int _rotation = 0;

        public event EventHandler<EventArgs> RotationChanged;

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

        public  async Task OpenBarrier()
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

        private void OnRotationChanged()
        {
            RotationChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task CloseBarrier()
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

        public Task PauseBarrier()
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }
    }
}
