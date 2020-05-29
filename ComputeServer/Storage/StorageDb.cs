using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace Compute.Storage
{
    public class StorageDb
    {
        public static void EnsureDatabase(string path, string template, Dictionary<string, string> tags)
        {
            if (!File.Exists(path))
                CreateDatabase(path, template, tags);
        }

        public static void CreateDatabase(string path, string template, Dictionary<string, string> tags)
        {
            try
            {
                if (!File.Exists(path))
                    File.Delete(path);
                var @base = GetContent(Assembly.GetExecutingAssembly().GetManifestResourceStream($"ComputeServer.Storage.base.sql"), template);
                var single = GetContent(Assembly.GetExecutingAssembly().GetManifestResourceStream($"ComputeServer.Storage.{template}.sql"), template);
                using (var cnn = new SQLiteConnection($@"Data Source={path}"))
                {
                    cnn.Open();
                    CreateDatabaseTables(cnn, @base, tags);
                    CreateDatabaseTables(cnn, single, tags);
                }
            }
            catch { File.Delete(path); throw; }
        }

        static void CreateDatabaseTables(SQLiteConnection cnn, string content, Dictionary<string, string> tags)
        {
            var queries = content.Split(new[] { "\nGO" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var query in queries)
            {
                var idx = query.IndexOf("--${");
                if (idx == -1 || idx > 2)
                {
                    cnn.Execute(query.Trim());
                    continue;
                }
                var idx2 = query.IndexOf("}", idx);
                if (idx2 == -1)
                    throw new InvalidOperationException("--${ found with out closing }");
                var newTags = query.Substring(idx + 4, idx2 - idx - 4).Split(',');
                var newQuery = query.Substring(idx2 + 1);
                foreach (var newTag in newTags)
                    newQuery = newQuery.Replace($"${newTag}", tags[newTag]);
                cnn.Execute(newQuery.Trim());
            }
        }

        static string GetContent(Stream stream, string template)
        {
            using (var sr = new StreamReader(stream ?? throw new ArgumentNullException(nameof(stream), $"template '{template}' not found")))
                return sr.ReadToEnd();
        }
    }
}
