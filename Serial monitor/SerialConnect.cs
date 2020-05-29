using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Controls;
using System.Timers;
using System.Diagnostics;

namespace Serial_monitor {
    class SerialConnect {
        public delegate void UpdatePortHandler(string[] port);
        public delegate void DataReceivedHandler(string data);

        public event UpdatePortHandler updatePort;
        public event DataReceivedHandler DataReceived;
        public event DataReceivedHandler error;

        private SerialPort _serialPort;
        //private MainWindow Box;

        private Timer timerS = new Timer();

        public SerialConnect() {
            //this.Box = Box;
            _serialPort = new SerialPort();
            _serialPort.BaudRate = 9600;
            _serialPort.PortName = "COM3";
            //_serialPort.DataReceived += _serialPort_DataReceived;
            timerS.Interval = 100;
            timerS.Elapsed += TimerS_Elapsed;
        }

        private void TimerS_Elapsed(object sender, ElapsedEventArgs e) {
            if (!_serialPort.IsOpen)
                return;

            byte[] data = new byte[_serialPort.BytesToRead];
            _serialPort.Read(data, 0, data.Length);
            string text = Encoding.Default.GetString(data);

            if (data.Length != 0)
                DataReceived(text);
            //Box.appendToConsoleOut(text);
        }

        /*private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            if (!_serialPort.IsOpen)
                return;
            
            //byte[] data = new byte[_serialPort.BytesToRead];
            //_serialPort.Read(data, 0, data.Length);
            //string text = Encoding.Default.GetString(data);

            string text = _serialPort.ReadExisting();
            //string text = _serialPort.ReadLine();

            Task.Factory.StartNew(() => {
                Box.appendToConsoleOut(text);
            });
        }*/

        public void SendData(string data) {
            if(_serialPort.IsOpen)
                _serialPort.Write(data);
        }

        public void SetBaudRate(int BaudRate) {
            _serialPort.BaudRate = BaudRate;
        }

        public void SetPortName(string PortName) {
            _serialPort.PortName = PortName;
        }

        public void Update() {
            string[] ports = SerialPort.GetPortNames();
            List<string> portList = new List<string>();

            foreach (string port in ports) {
                if(portList.IndexOf(port) == -1) {
                    portList.Add(port);
                }
            }

            updatePort(portList.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Kunne den forbinde</returns>
        public bool Open() {
            bool isOK = false;
            try {
                if (!_serialPort.IsOpen) {
                    _serialPort.Open();
                }
                timerS.Start();
                isOK = true;
            } catch {
                error("!!! CAN'T CONNECT !!!\n\r");
                //Box.appendToConsoleOut("!!! CAN'T CONNECT !!!\n\r");
            }

            return isOK;
        }

        public void Close() {
            timerS.Stop();
            _serialPort.Close();
        }
    }
}
