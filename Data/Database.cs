// Połączenie z bazą danych

using System.Configuration;
using System.Diagnostics;
using Npgsql;
using projekt.Interfaces;

namespace projekt.Data
{
    public class Database : IDatabase
    {
        private string _connectionString;

        public Database()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PostgresConnection"].ConnectionString;
        }

        public NpgsqlConnection GetConnection()
        {
            try
            {
                var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                return connection;
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }

        }
    }
}
