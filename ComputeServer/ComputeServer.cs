using Compute.Repository;
using Compute.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Tdstream.Server;

namespace Compute
{
    public interface IComputeServer
    {
        bool LoginHandler(FreeTds.TdsLogin login);
        void QueryHandler(TdsContext ctx);
        void Run();
    }

    public class ComputeServer : IComputeServer
    {
        readonly IMasterFile _masterFile;

        public ComputeServer(IMasterFile masterFile)
        {
            _masterFile = masterFile;
            Refresh();
        }

        public void Refresh()
        {
            Databases = _masterFile.GetDatabases().Select(x => x.Refresh(_masterFile)).ToList();
            Servers = _masterFile.GetServers();
            Principals = _masterFile.GetServerPrincipals().Select(x => x.Refresh(_masterFile)).ToList();
        }

        public ICollection<sys_database> Databases { get; private set; }
        
        public ICollection<sys_server> Servers { get; private set; }

        public ICollection<sys_server_principal> Principals { get; private set; }

        public bool LoginHandler(FreeTds.TdsLogin login)
        {
            return true;
            //login.UserName != "guest" && login.Password != "sybase"
        }

        const string FirstQuery = @"DECLARE @edition sysname;
SET @edition = cast(SERVERPROPERTY(N'EDITION') as sysname);
SELECT case when @edition = N'SQL Azure' then 2 else 1 end as 'DatabaseEngineType',
SERVERPROPERTY('EngineEdition') AS DatabaseEngineEdition,
SERVERPROPERTY('ProductVersion') AS ProductVersion,
@@MICROSOFTVERSION AS MicrosoftVersion;
select N'Windows' as host_platform
if @edition = N'SQL Azure' 
  select 'TCP' as ConnectionProtocol
else
  exec ('select CONVERT(nvarchar(40),CONNECTIONPROPERTY(''net_transport'')) as ConnectionProtocol')
";

        public bool StandardQueryHandler(TdsContext ctx)
        {
            var res = ctx.Response;
            switch (ctx.Request.Query)
            {
                case FirstQuery:
                    // first table
                    res.NewTable(4)
                        .Column(0, "DatabaseEngineType", typeof(int), 4)
                        .Column(1, "DatabaseEngineEdition", typeof(string), 30)
                        .Column(2, "ProductVersion", typeof(string), 30)
                        .Column(3, "MicrosoftVersion", typeof(int), 4);
                    res.NewRow()
                        .ColumnData(0, 1)
                        .ColumnData(1, "2")
                        .ColumnData(2, "11.0.7493.4")
                        .ColumnData(3, 184556869);
                    // second table
                    res.NewTable(3)
                        .Column(0, "host_platform", typeof(string), 7);
                    res.NewRow()
                        .ColumnData(0, "Windows");
                    // third table
                    res.NewTable(1)
                        .Column(0, "ConnectionProtocol", typeof(string), 3);
                    res.NewRow()
                        .ColumnData(0, "TCP");
                    res.Done();
                    return true;
            }
            return false;
        }

        public void QueryHandler(TdsContext ctx)
        {
            if (StandardQueryHandler(ctx))
                return;

            var res = ctx.Response;
            res.NewTable(1)
                .Column(0, "content", typeof(string), 30);
            res.NewRow()
                .ColumnData(0, "content");
            var info = res.Info;
            info.Columns[0].ColumnData = Marshal.StringToHGlobalAnsi("content");
            res.Done();
        }

        public void Run()
        {
        }
    }
}
