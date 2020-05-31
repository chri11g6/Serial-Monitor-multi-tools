using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Windows;

namespace Serial_monitor {
    public static class CSV {
        public static void Export(ObservableCollection<ConsoleTableClass> dataList) {
            List<ConsoleTableClass> datas = dataList.ToList();
            StringBuilder csv = new StringBuilder();

            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "CSV|*csv|All Files|*.*";
            sfd.Title = "Save table data";
            sfd.FileName = "Data.csv";

            if ((bool)sfd.ShowDialog()) {
                csv.AppendLine("ID;Context;Timestamp;");
                foreach (ConsoleTableClass data in datas) {
                    csv.AppendLine(data.ID.ToString() + ";" + data.Context + ";" + data.Timestamp + ";");
                }
                File.WriteAllText(sfd.FileName, csv.ToString());
            }
        }
        
        public static void Export(ObservableCollection<TempTableClass> dataList) {
            List<TempTableClass> datas = dataList.ToList();
            StringBuilder csv = new StringBuilder();

            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "CSV|*csv|All Files|*.*";
            sfd.Title = "Save Temp table data";
            sfd.FileName = "TempData.csv";

            if ((bool)sfd.ShowDialog()) {
                csv.AppendLine("ID;From;To;");
                foreach (TempTableClass data in datas) {
                    csv.AppendLine(data.ID.ToString() + ";" + data.From + ";" + data.To + ";");
                }
                File.WriteAllText(sfd.FileName, csv.ToString());
            }
        }

        public static void Import(ref ObservableCollection<TempTableClass> dataList) {
            List<TempTableClass> datas = new List<TempTableClass>();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV|*csv|All Files|*.*";
            ofd.Title = "Load Temp data";

            try {
                if ((bool)ofd.ShowDialog()) {
                    string[] dataLine = File.ReadAllText(ofd.FileName).Split('\n');

                    bool isFristLine = true;

                    foreach(string line in dataLine) {
                        if (string.IsNullOrEmpty(line))
                            continue;

                        string[] data = line.Split(';');

                        if (isFristLine) {
                            try {
                                int talx = int.Parse(data[0]);
                            } catch {
                                continue;
                            }
                        }


                        datas.Add(new TempTableClass { ID = int.Parse(data[0]), From = data[1], To = data[2] });
                    }

                    dataList.Clear();

                    foreach(TempTableClass temp in datas) {
                        dataList.Add(temp);
                    }
                }
            } catch (Exception e) {
                StringBuilder message = new StringBuilder();

                message.AppendLine(e.Message);

                message.AppendLine("Can't import this CSV file");

                MessageBox.Show(message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
