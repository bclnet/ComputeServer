using System;
using System.Diagnostics;

namespace Compute.Storage
{
    // Select * From sys.objects
    [DebuggerDisplay("{name}")]
    public class sys_object
    {
        public string name { get; set; }
        public int object_id { get; set; }
        public int schema_id { get; set; }
        public string type { get; set; }
        public DateTime create_date { get; set; }
        public DateTime modify_date { get; set; }
    }
}
