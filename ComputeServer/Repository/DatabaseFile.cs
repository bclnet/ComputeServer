using Compute.Storage;
using Dapper;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Compute.Repository
{
    public interface IDatabaseFile
    {
        // sys_database_principal
        bool SaveDatabasePrincipal(sys_database_principal principal);
        sys_database_principal GetDatabasePrincipalById(int id);
        ICollection<sys_database_principal> GetDatabasePrincipals();
        // sys_database_role_member
        ICollection<sys_database_role_member> GetDatabaseRoleMembers(sys_database_principal principal);
        // sys_schema
        bool SaveSchema(sys_schema schema);
        sys_schema GetSchemaById(int id);
        ICollection<sys_schema> GetSchemas();
        // sys_object
        bool SaveObject(sys_object obj);
        sys_object GetObjectById(int id);
        ICollection<sys_object> GetObjects();
    }

    public abstract class DatabaseFile : IDatabaseFile
    {
        public abstract string DbTemplate { get; }
        public abstract string DbPath { get; }

        public SQLiteConnection Connection()
        {
            StorageDb.EnsureDatabase(DbPath, DbTemplate, new Dictionary<string, string> { { "folder", Path.GetDirectoryName(DbPath) } });
            var cnn = new SQLiteConnection($@"Data Source={DbPath}");
            cnn.Open();
            return cnn;
        }

        #region sys_database_principal

        public bool SaveDatabasePrincipal(sys_database_principal principal)
        {
            using (var cnn = Connection())
                return principal.principal_id == 0
                    ? (principal.principal_id = (int)cnn.Query<long>(@"
INSERT INTO sys_database_principal (name, sid, type, default_schema_name)
VALUES (@name, @sid, @type, @default_schema_name);
SELECT last_insert_rowid();", principal).First()) != 0
                    : cnn.Query<int>(@"
UPDATE sys_database_principal
SET name = @name, sid = @sid, type = @type, default_schema_name = @default_schema_name, modify_date = datetime()
WHERE principal_id = @principal_id;
SELECT changes();", principal).First() != 0;
        }

        public sys_database_principal GetDatabasePrincipalById(int id)
        {
            using (var cnn = Connection())
                return cnn.Query<sys_database_principal>(@"
SELECT name, principal_id, sid, type, default_schema_name, create_date, modify_date
FROM sys_database_principal
WHERE principal_id = @id", new { id }).FirstOrDefault();
        }

        public ICollection<sys_database_principal> GetDatabasePrincipals()
        {
            using (var cnn = Connection())
                return cnn.Query<sys_database_principal>(@"
SELECT name, principal_id, sid, type, default_schema_name, create_date, modify_date
FROM sys_database_principal").ToList();
        }

        #endregion

        #region sys_database_role_member

        public ICollection<sys_database_role_member> GetDatabaseRoleMembers(sys_database_principal principal)
        {
            using (var cnn = Connection())
                return cnn.Query<sys_database_role_member>(@"
SELECT role_principal_id, member_principal_id
FROM sys_database_role_member
WHERE role_principal_id = @principal_id", new { principal.principal_id }).ToList();
        }

        #endregion

        #region sys_schema

        public bool SaveSchema(sys_schema schema)
        {
            using (var cnn = Connection())
                return schema.schema_id == 0
                    ? (schema.schema_id = (int)cnn.Query<long>(@"
INSERT INTO sys_schema (name, princial_id)
VALUES (@name, @princial_id);
SELECT last_insert_rowid();", schema).First()) != 0
                    : cnn.Query<int>(@"
UPDATE sys_schema
SET name = @name, princial_id = @princial_id
WHERE schema_id = @schema_id;
SELECT changes();", schema).First() != 0;
        }

        public sys_schema GetSchemaById(int id)
        {
            using (var cnn = Connection())
                return cnn.Query<sys_schema>(@"
SELECT name, schema_id, princial_id
FROM sys_schema
WHERE schema_id = @id", new { id }).FirstOrDefault();
        }

        public ICollection<sys_schema> GetSchemas()
        {
            using (var cnn = Connection())
                return cnn.Query<sys_schema>(@"
SELECT name, schema_id, princial_id
FROM sys_schema").ToList();
        }

        #endregion

        #region sys_object

        public bool SaveObject(sys_object obj)
        {
            using (var cnn = Connection())
                return obj.object_id == 0
                    ? (obj.object_id = (int)cnn.Query<long>(@"
INSERT INTO sys_object (name, schema_id, type)
VALUES (@name, @schema_id, @type);
SELECT last_insert_rowid();", obj).First()) != 0
                    : cnn.Query<int>(@"
UPDATE sys_object
SET name = @name, schema_id = @schema_id, type = @type
WHERE object_id = @object_id;
SELECT changes();", obj).First() != 0;
        }

        public sys_object GetObjectById(int id)
        {
            using (var cnn = Connection())
                return cnn.Query<sys_object>(@"
SELECT name, object_id, schema_id, type, create_date, modify_date
FROM sys_object
WHERE object_id = @id", new { id }).FirstOrDefault();
        }

        public ICollection<sys_object> GetObjects()
        {
            using (var cnn = Connection())
                return cnn.Query<sys_object>(@"
SELECT name, object_id, schema_id, type, create_date, modify_date
FROM sys_object").ToList();
        }

        #endregion
    }
}
