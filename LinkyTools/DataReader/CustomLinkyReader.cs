using LinkyTools.Linky;

namespace LinkyTools.DataReader
{
    public class ReceivedLinkyDataEventArgs : EventArgs
    {
        public LinkyStandardData? LinkyStandardData { get; set; }
    }

    public abstract class CustomLinkyReader
    {
        #region Events
        public event EventHandler<ReceivedLinkyDataEventArgs>? OnReceivedData;
        #endregion

        #region Fields
        private static readonly object SerialBufferLock = new();
        private readonly LinkyStandardData? _LinkyStandardData;
        public LinkyStandardData LinkyStandardData { get => _LinkyStandardData!; }
        private PeriodicTimer? _PeriodicTimerSendLinkyDatas;

        protected bool _IsActive;

        public bool IsActive { get => _IsActive; set => SetIsActive(value); }

        protected void SetIsActive(bool value)
        {
            if (_IsActive != value)
            {
                _IsActive = value;
                if (value)
                {
                    StartReadingLinkyInformations();
                }
                else
                {
                    StopReadingLinkyInformations();
                }
            }
        }
        #endregion

        #region Ctor
        public CustomLinkyReader()
        {
            _LinkyStandardData = new LinkyStandardData();
        }
        #endregion

        protected virtual void StartReadingLinkyInformations()
        {
            _IsActive = true;
            StartTimerReadBuffer();
        }

        private async void StartTimerReadBuffer()
        {
            var e = new ReceivedLinkyDataEventArgs();
            _PeriodicTimerSendLinkyDatas = new PeriodicTimer(TimeSpan.FromSeconds(2));
            while (await _PeriodicTimerSendLinkyDatas.WaitForNextTickAsync())
            {
                e.LinkyStandardData = _LinkyStandardData;
                OnReceivedData?.Invoke(this, e);
            }
        }

        protected virtual void StopReadingLinkyInformations()
        {
            _PeriodicTimerSendLinkyDatas!.Dispose();
            _IsActive = false;
        }

        protected void NewDataReceived(in string datas)
        {
            lock (SerialBufferLock)
            {
                _LinkyStandardData!.AppendRawDatas(datas);
            }
        }

    }
}
