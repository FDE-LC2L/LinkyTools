using LinkyTools.Linky;
using System.IO.Ports;

namespace LinkyTools.Serial
{
    public class ReceivedLinkyDataEventArgs : EventArgs
    {
        public LinkyStandardData? LinkyStandardData { get; set; }
    }

    public class LinkySerialReader
    {
        public event EventHandler<ReceivedLinkyDataEventArgs>? OnReceivedData;

        private readonly SerialPort? _SerialPort;
        private PeriodicTimer? _PeriodicTimerReadBuffer;
        private readonly LinkyStandardData? _LinkyStandardData;
        private static readonly object SerialBufferLock = new();
        public LinkyStandardData LinkyStandardData { get => _LinkyStandardData!; }

        public LinkySerialReader()
        {
            _SerialPort = new SerialPort
            {
                PortName = "COM3", // Adapt this value to your configuration 
                BaudRate = 9600,
                Parity = Parity.Even,
                StopBits = StopBits.One, 
                DataBits = 7
            };
            _SerialPort.DataReceived += SerialPort_DataReceived;
            _LinkyStandardData = new LinkyStandardData();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (SerialBufferLock)
            {
                LinkyStandardData!.AppendRawDatas(_SerialPort!.ReadExisting());
            }
        }

        public async void StartReadLinkyInformations()
        {
            var e = new ReceivedLinkyDataEventArgs();
            _SerialPort!.Open();
            _PeriodicTimerReadBuffer = new PeriodicTimer(TimeSpan.FromSeconds(2));
            while (await _PeriodicTimerReadBuffer.WaitForNextTickAsync())
            {
                e.LinkyStandardData = _LinkyStandardData;
                OnReceivedData?.Invoke(this, e);
            }
        }

        public void StopReadLinkyInformations()
        {
            _SerialPort!.Close();
            _PeriodicTimerReadBuffer!.Dispose();
        }


    }
}
