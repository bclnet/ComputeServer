using System.Diagnostics;

namespace Compute.Storage
{
    // Select * From sys.schemas
    [DebuggerDisplay("{name}")]
    public class sys_schema
    {
        public string name { get; set; }
        public int schema_id { get; set; }
        public int princial_id { get; set; }
    }
}
