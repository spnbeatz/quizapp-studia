using Npgsql;

namespace projekt.Interfaces
{
    public interface IDatabase
    {
        NpgsqlConnection GetConnection();
    }
}
