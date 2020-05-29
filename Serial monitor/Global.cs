using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_monitor {
    public static class Global {
        private static SerialConnect sC = new SerialConnect();
        public static List<RX> dataRX = new List<RX>();

        internal static SerialConnect SC { get => sC; set => sC = value; }
    }
}
