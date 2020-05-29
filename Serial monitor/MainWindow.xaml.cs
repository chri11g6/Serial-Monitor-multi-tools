using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace Serial_monitor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        //private SerialConnect SC;
        private RX rx = new RX();
        public List<string> PortCom { get; set; }

        private int[] BaudRateArray = { 300,1200,2400,4800,9600,19200,38400,57600,74880,115200,230400,250000,500000,1000000,2000000 };
        private ObservableCollection<TempTableClass> TempTables = new ObservableCollection<TempTableClass>();
        private ObservableCollection<ConsoleTableClass> ConsoleTables = new ObservableCollection<ConsoleTableClass>();


        public MainWindow() {
            InitializeComponent();
            DataGridTempTable.ItemsSource = TempTables;
            DataGridConsoleTable.ItemsSource = ConsoleTables;

            PortCom = new List<string>();

            Global.SC.updatePort += (string[] ports) => {
                PortCom.Clear();
                foreach(string port in ports) {
                    PortCom.Add(port);
                }

                if(PortCom.Count == 0) {
                    PortCom.Add("COM");
                }

                ComboBoxPort.ItemsSource = PortCom;
            };

            Global.SC.DataReceived += (string data) => {
                appendToConsoleOut(data);
            };

            Global.SC.Update();
            // Selected kun i start ved ComboBox Port
            ComboBoxPort.SelectedIndex = 0;
            
        }

        #region Button function

        private void SaveData_Click(object sender, RoutedEventArgs e) {
            SaveDataF();
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e) {
            sendData();
        }

        private void ClearConsoleOut() {
            ConsoleOut.Text = "";
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e) {
            ClearConsoleOut();
        }

        private void MenuButtonClear_Click(object sender, RoutedEventArgs e) {
            ClearConsoleOut();
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e) {
            string open = "Connect";
            string close = "Disconnect";

            string val = buttonConnect.Content.ToString();

            if(val == open) {
                Global.SC.SetPortName(ComboBoxPort.SelectedItem.ToString());
                Global.SC.SetBaudRate(BaudRateArray[ComboBoxBaud.SelectedIndex]);

                if (!Global.SC.Open())
                    return;

                buttonConnect.Content = close;
            } else if (val == close) {
                Global.SC.Close();

                buttonConnect.Content = open;
            }
        }

        private void Test_Click(object sender, RoutedEventArgs e) {
            foreach (RX x in Global.dataRX) {
                Debug.WriteLine(x.Data);
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Global.SC.Close();
            Application.Current.Shutdown();
        }
        #endregion

        #region Backgroud function

        private void sendData() {
            string text = ConsoleIn.Text;

            switch (ComboBoxSendType.SelectedIndex) {
                case 0:
                    //noting
                    break;
                case 1:
                    text += "\n";
                    break;
                case 2:
                    text += "\r";
                    break;
                case 3:
                    text += "\n\r";
                    break;
            }

            Global.SC.SendData(text);

            //appendToConsoleOut(text);
            ConsoleIn.Text = "";
        }

        // Skal kigge på.
        private void UpdateRX(string data) {

            string[] x = Regex.Split(data, "\r\n");

            for (int i = 0; i < x.Length; i++) {
                if (x.Length > 1 && i != (x.Length - 1)) {
                    Debug.WriteLine("OK Send: " + x[i]);
                    rx.Data += x[i];
                    rx.EndTime = DateTime.Now;
                    if (rx.Data != "")
                        Global.dataRX.Add(rx);
                    rx = new RX();
                    //rx = null;
                } else {
                    Debug.WriteLine("Add to: " + x[i]);
                    rx.Data += x[i];
                }
            }

        }

        private void SaveDataF() {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save data";
            sfd.Filter = "Text Files|*.txt|All Files|*.*";
            sfd.FileName = "Data";

            if ((bool)sfd.ShowDialog()) {
                File.WriteAllText(sfd.FileName, ConsoleOut.Text);
            }
        }

        public void appendToConsoleOut(string text) {
            Dispatcher.BeginInvoke(new Action(() => {

                UpdateRX(text);

                if ((bool)timestampCheck.IsChecked) {
                    ConsoleOut.Text += DateTime.Now.ToString("HH:mm:ss.ffff") + " -> " + text;
                    //ConsoleOut.Text += DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffff") + " -> " + text;
                } else {
                    ConsoleOut.Text += text;
                }

                if ((bool)AutoscrollCheck.IsChecked) {
                    ConsoleOutScroll.ScrollToEnd();
                }
            }));
        }
        #endregion

        private void ComboBoxPort_DropDownOpened(object sender, EventArgs e) {
            Global.SC.Update();
            Debug.WriteLine("Com Update");
            //Debug.WriteLine(ComboBoxPort.SelectedItem);
        }

        private void ConsoleIn_KeyUp(object sender, KeyEventArgs e) {
            if(e.Key == Key.Enter) {
                sendData();
            }
        }
        
    }

    public class TempTableClass {
        public int ID { set; get; }
        public string Text { set; get; }
    }

    public class ConsoleTableClass {
        public int ID { set; get; }
        public string Context { set; get; }
        public string Timestamp { set; get; }
    }
}
