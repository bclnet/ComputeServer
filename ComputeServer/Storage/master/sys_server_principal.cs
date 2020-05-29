using Compute.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Compute.Storage
{
    // Select * From sys.server_principals
    [DebuggerDisplay("{name}")]
    public class sys_server_principal
    {
        public string name { get; set; }
        public int principal_id { get; set; }
        public byte[] sid { get; set; }
        public string type { get; set; }
        public string type_desc => $"{type}";
        public bool is_disabled { get; set; }
        public DateTime create_date { get; set; }
        public DateTime modify_date { get; set; }
        public string default_database_name { get; set; }
        public string default_language_name { get; set; }

        public sys_server_principal Refresh(IMasterFile masterFile)
        {
            RoleMembers = type == "R" ? masterFile.GetServerRoleMembers(this) : null;
            Permissions = masterFile.GetServerPermissions(this);
            return this;
        }

        public ICollection<sys_server_role_member> RoleMembers { get; private set; }

        public ICollection<sys_server_permission> Permissions { get; private set; }
    }
}
