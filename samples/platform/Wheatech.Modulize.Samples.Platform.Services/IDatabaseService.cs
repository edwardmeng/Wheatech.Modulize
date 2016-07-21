using System.Data;

namespace Wheatech.Modulize.Samples.Platform.Services
{
    public interface IDatabaseService
    {
        int ExecuteNonQuery(string commandText);

        IDataReader ExecuteReader(string commandText);

        object ExecuteScalar(string commandText);
    }
}
