using Compute.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Compute.Storage
{
    // Select * From sys.database_principals
    [DebuggerDisplay("{name}")]
    public class sys_database_principal
    {
        public string name { get; set; }
        public int principal_id { get; set; }
        public byte[] sid { get; set; }
        public string type { get; set; }
        public string type_desc => $"{type}";
        public string default_schema_name { get; set; }
        public DateTime create_date { get; set; }
        public DateTime modify_date { get; set; }

        public sys_database_principal Refresh(IDatabaseFile databaseFile)
        {
            RoleMembers = type == "R" ? databaseFile.GetDatabaseRoleMembers(this) : null;
            return this;
        }

        public ICollection<sys_database_role_member> RoleMembers { get; private set; }
    }
}
