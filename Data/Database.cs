// Połączenie z bazą danych

using System.Configuration;
using Npgsql;
using projekt.Interfaces;

namespace projekt.Data
{
    public class Database : IDatabase
    {
        private readonly string _connectionString;

        public Database()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PostgresConnection"].ConnectionString;
        }

        public NpgsqlConnection GetConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
