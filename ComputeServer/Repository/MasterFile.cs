using Compute.Storage;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using static Compute.Compute;

namespace Compute.Repository
{
    public interface IMasterFile : IDatabaseFile
    {
        // sys_database
        bool SaveDatabase(sys_database database);
        sys_database GetDatabaseById(int id);
        ICollection<sys_database> GetDatabases();
        // sys_database_file
        bool SaveDatabaseFile(sys_database_file file);
        sys_database_file GetDatabaseFileById(int id);
        ICollection<sys_database_file> GetDatabaseFiles(sys_database database);
        // sys_server
        bool SaveServer(sys_server server);
        sys_server GetServerById(int id);
        ICollection<sys_server> GetServers();
        // sys_server_principal
        bool SaveServerPrincipal(sys_server_principal principal);
        sys_server_principal GetServerPrincipalById(int id);
        ICollection<sys_server_principal> GetServerPrincipals();
        // sys_server_role_member
        ICollection<sys_server_role_member> GetServerRoleMembers(sys_server_principal principal);
        // sys_server_permission
        ICollection<sys_server_permission> GetServerPermissions(sys_server_principal principal);
    }

    public class MasterFile : DatabaseFile, IMasterFile
    {
        public override string DbTemplate => "master";
        public override string DbPath => $@"{DataPath}\master.db";

        #region sys_database

        public bool SaveDatabase(sys_database database)
        {
            using (var cnn = Connection())
                return database.database_id == 0
                    ? (database.database_id = (int)cnn.Query<long>(@"
INSERT INTO sys_database (name, recovery_model)
VALUES (@name, @recovery_model);
SELECT last_insert_rowid();", database).First()) != 0
                    : cnn.Query<int>(@"
UPDATE sys_database
SET name = @name, recovery_model = @recovery_model
WHERE database_id = @database_id;
SELECT changes();", database).First() != 0;
        }

        public sys_database GetDatabaseById(int id)
        {
            using (var cnn = Connection())
                return cnn.Query<sys_database>(@"
SELECT name, database_id, create_date, recovery_model
FROM sys_database
WHERE principal_id = @id", new { id }).FirstOrDefault();
        }

        public ICollection<sys_database> GetDatabases()
        {
            using (var cnn = Connection())
                return cnn.Query<sys_database>(@"
SELECT name, database_id, create_date, recovery_model
FROM sys_database").ToList();
        }

        #endregion

        #region sys_database_file

        public bool SaveDatabaseFile(sys_database_file file)
        {
            using (var cnn = Connection())
                return file.file_id == 0
                    ? (file.file_id = (int)cnn.Query<long>(@"
INSERT INTO sys_database_file (database_id, type, name, physical_name, state)
VALUES (@database_id, @type, @name, @physical_name, @state);
SELECT last_insert_rowid();", file).First()) != 0
                    : cnn.Query<int>(@"
UPDATE sys_database_file
SET database_id = @database_id, type = @type, name = @name, physical_name = @physical_name, state = @state
WHERE file_id = @file_id;
SELECT changes();", file).First() != 0;
        }

        public sys_database_file GetDatabaseFileById(int id)
        {
            using (var cnn = Connection())
                return cnn.Query<sys_database_file>(@"
SELECT file_id, database_id, type, name, physical_name, state
FROM sys_database_file
WHERE file_id = @id", new { id }).FirstOrDefault();
        }

        public ICollection<sys_database_file> GetDatabaseFiles(sys_database database)
        {
            using (var cnn = Connection())
                return cnn.Query<sys_database_file>(@"
SELECT file_id, database_id, type, name, physical_name, state
FROM sys_database_file
WHERE database_id = @database_id", new { database.database_id }).ToList();
        }

        #endregion

        #region sys_server

        public bool SaveServer(sys_server server)
        {
            using (var cnn = Connection())
                return server.server_id == 0
                    ? (server.server_id = (int)cnn.Query<long>(@"
INSERT INTO sys_server (name, provider, data_source)
VALUES (@name, @provider, @data_source);
SELECT last_insert_rowid();", server).First()) != 0
                    : cnn.Query<int>(@"
UPDATE sys_server
SET name = @name, provider = @provider, data_source = @data_source, modify_date = datetime()
WHERE database_id = @database_id;
SELECT changes();", server).First() != 0;
        }

        public sys_server GetServerById(int id)
        {
            using (var cnn = Connection())
                return cnn.Query<sys_server>(@"
SELECT server_id, name, provider, data_source, modify_date
FROM sys_server
WHERE server_id = @id", new { id }).FirstOrDefault();
        }

        public ICollection<sys_server> GetServers()
        {
            using (var cnn = Connection())
                return cnn.Query<sys_server>(@"
SELECT server_id, name, provider, data_source, modify_date
FROM sys_server").ToList();
        }

        #endregion

        #region sys_server_principal

        public bool SaveServerPrincipal(sys_server_principal principal)
        {
            using (var cnn = Connection())
                return principal.principal_id == 0
                    ? (principal.principal_id = (int)cnn.Query<long>(@"
INSERT INTO sys_server_principal (name, sid, type, is_disabled, default_database_name, default_language_name)
VALUES (@name, @sid, @type, @is_disabled, @default_database_name, @default_language_name);
SELECT last_insert_rowid();", principal).First()) != 0
                    : cnn.Query<int>(@"
UPDATE sys_server_principal
SET name = @name, sid = @sid, type = @type, is_disabled = @is_disabled, modify_date = datetime(), default_database_name = @default_database_name, default_language_name = @default_language_name
WHERE principal_id = @principal_id;
SELECT changes();", principal).First() != 0;
        }

        public sys_server_principal GetServerPrincipalById(int id)
        {
            using (var cnn = Connection())
                return cnn.Query<sys_server_principal>(@"
SELECT name, principal_id, sid, type, is_disabled, create_date, modify_date, default_database_name, default_language_name
FROM sys_server_principal
WHERE principal_id = @id", new { id }).FirstOrDefault();
        }

        public ICollection<sys_server_principal> GetServerPrincipals()
        {
            using (var cnn = Connection())
                return cnn.Query<sys_server_principal>(@"
SELECT name, principal_id, sid, type, is_disabled, create_date, modify_date, default_database_name, default_language_name
FROM sys_server_principal").ToList();
        }

        #endregion

        #region sys_server_role_member

        public ICollection<sys_server_role_member> GetServerRoleMembers(sys_server_principal principal)
        {
            using (var cnn = Connection())
                return cnn.Query<sys_server_role_member>(@"
SELECT role_principal_id, member_principal_id
FROM sys_server_role_member
WHERE role_principal_id = @principal_id", new { principal.principal_id }).ToList();
        }

        #endregion

        #region sys_server_permission

        public ICollection<sys_server_permission> GetServerPermissions(sys_server_principal principal)
        {
            using (var cnn = Connection())
                return cnn.Query<sys_server_permission>(@"
SELECT class, grantee_principal_id, grantor_principal_id, type, state
FROM sys_server_permission
WHERE grantee_principal_id = @principal_id", new { principal.principal_id }).ToList();
        }

        #endregion
    }
}
