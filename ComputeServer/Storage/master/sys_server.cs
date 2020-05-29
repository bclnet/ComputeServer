using System;
using System.Diagnostics;

namespace Compute.Storage
{
    // Select * From sys.servers
    [DebuggerDisplay("{name}")]
    public class sys_server
    {
        public int server_id { get; set; }
        public string name { get; set; }
        public string provider { get; set; }
        public string data_source { get; set; }
        public DateTime modify_date { get; set; }
    }
}
