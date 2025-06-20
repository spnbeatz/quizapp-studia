// klasa serwisu zarzadzajaca logika uzytkownika

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using projekt.Data;
using projekt.Interfaces;
using projekt.Models;
using projekt.Utils;

namespace projekt.Services
{
    internal class UserService : IUserService
    {
        private readonly IDatabase _database;
        private readonly NpgsqlConnection _conn;

        public UserService(IDatabase database)
        {
            _database = database;
            _conn = _database.GetConnection();
        }

        public User? AuthenticateUser(string username, string password)
        {


            var cmd = new NpgsqlCommand("SELECT id, username FROM users WHERE username = @username AND password = @password", _conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("password", password);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1)
                };
            }

            return null;
        }

        public string? GetUserPasswordHash(string username)
        {
            using var cmd = new NpgsqlCommand("SELECT password FROM users WHERE username = @username", _conn);
            cmd.Parameters.AddWithValue("username", username);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetString(0);
            }

            return null;
        }

        public bool RegisterUser(string username, string password)
        {
            var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @username", _conn);
            checkCmd.Parameters.AddWithValue("username", username);
            var exists = (long)checkCmd.ExecuteScalar();

            if (exists > 0)
                return false;

            var cmd = new NpgsqlCommand("INSERT INTO users (username, password) VALUES (@username, @password)", _conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("password", password);

            return cmd.ExecuteNonQuery() == 1;
        }

        public User? GetUserById(int userId)
        {

            var cmd = new NpgsqlCommand("SELECT id, username FROM users WHERE id = @id", _conn);
            cmd.Parameters.AddWithValue("id", userId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1)
                };
            }

            return null;
        }

        public void Dispose()
        {
            _conn?.Dispose();
        }
    }
}
