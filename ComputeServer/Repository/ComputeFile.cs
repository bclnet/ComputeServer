namespace Compute.Repository
{
    public interface IComputeFile : IDatabaseFile
    {
    }

    public class ComputeFile : DatabaseFile, IComputeFile
    {
        public override string DbTemplate => "model";
        public override string DbPath => _dbPath;

        readonly string _dbPath;

        public ComputeFile(string dbPath) => _dbPath = dbPath;
    }
}
