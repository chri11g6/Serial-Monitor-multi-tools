using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_monitor {
    public class TempTableClass {
        public int ID { set; get; }
        public string From { set; get; }
        public string To { set; get; }
    }

    public class ConsoleTableClass {
        public int ID { set; get; }
        public string Context { set; get; }
        public string Timestamp { set; get; }
    }
}
