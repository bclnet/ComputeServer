using System.Diagnostics;

namespace Compute.Storage
{
    // Select * From sys.server_role_members
    [DebuggerDisplay("{role_principal_id}:{member_principal_id}")]
    public class sys_server_role_member
    {
        public int role_principal_id { get; set; }
        public int member_principal_id { get; set; }
    }
}
