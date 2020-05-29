using Compute.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Compute.Storage
{
    // Select * From sys.databases
    [DebuggerDisplay("{name}")]
    public class sys_database
    {
        public string name { get; set; }
        public int database_id { get; set; }
        public DateTime create_date { get; set; }
        public int recovery_model { get; set; }
        public string recovery_model_desc => $"{recovery_model}";

        public sys_database Refresh(IMasterFile masterFile)
        {
            Files = masterFile.GetDatabaseFiles(this);
            return this;
        }

        public ICollection<sys_database_file> Files { get; private set; }
    }
}
