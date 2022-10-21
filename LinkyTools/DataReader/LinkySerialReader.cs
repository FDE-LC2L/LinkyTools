using System.IO.Ports;

namespace LinkyTools.DataReader
{

    public class LinkySerialReader : CustomLinkyReader
    {
        private readonly SerialPort? _SerialPort;

        public LinkySerialReader() : base()
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

        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            NewDataReceived(_SerialPort!.ReadExisting());
        }

        protected override void StartReadingLinkyInformations()
        {
            base.StartReadingLinkyInformations();
            _SerialPort!.Open();

        }

        protected override void StopReadingLinkyInformations()
        {
            _SerialPort!.Close();
            base.StopReadingLinkyInformations();

        }


    }
}
