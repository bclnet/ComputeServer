using System.Diagnostics;

namespace Compute.Storage
{
    // Select * From sys.database_files
    [DebuggerDisplay("{physical_name}")]
    public class sys_database_file
    {
        public int file_id { get; set; }
        public int database_id { get; set; }
        public int type { get; set; }
        public string type_desc => $"{type}";
        public string name { get; set; }
        public string physical_name { get; set; }
        public int state { get; set; }
    }
}
