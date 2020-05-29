using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_monitor {
    [Serializable]
    public class RX {
        public DateTime StarTime { set; get; }
        public DateTime EndTime { set; get; }
        public string Data { set; get; }

        public RX() {
            StarTime = DateTime.Now;
        }
    }
}
