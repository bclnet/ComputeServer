using Compute.Repository;
using Compute.Storage;
using System.Collections.Generic;
using System.Linq;

namespace Compute
{
    public class ComputeDatabase
    {
        readonly IDatabaseFile _databaseFile;

        public ComputeDatabase(IDatabaseFile databaseFile)
        {
            _databaseFile = databaseFile;
            Refresh();
        }

        public void Refresh()
        {
            Principals = _databaseFile.GetDatabasePrincipals().Select(x => x.Refresh(_databaseFile)).ToList();
        }

        public ICollection<sys_database_principal> Principals { get; private set; }
    }
}
