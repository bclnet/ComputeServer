using System.Diagnostics;

namespace Compute.Storage
{
    // Select * From sys.server_permissions
    [DebuggerDisplay("{type}")]
    public class sys_server_permission
    {
        public int @class { get; set; }
        public string class_desc => $"{@class}";
        public int grantee_principal_id { get; set; }
        public int grantor_principal_id { get; set; }
        public string type { get; set; }
        public string permission_name => $"{type}";
        public string state { get; set; }
        public string state_desc => $"{state}";
    }
}
