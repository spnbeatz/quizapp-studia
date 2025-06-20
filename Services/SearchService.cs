using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using projekt.Interfaces;
using projekt.Models;

namespace projekt.Services
{
    internal class SearchService : ISearchService
    {
        private readonly IDatabase _database;
        private readonly NpgsqlConnection _conn;

        public SearchService(IDatabase database)
        {
            _database = database;
            _conn = _database.GetConnection();
        }

        public async Task<List<SearchIndexItem>> SearchAsync(string query, string? entityType = null)
        {
            var results = new List<SearchIndexItem>();

            var sql = @"
                SELECT id, entity_type, entity_id, content
                FROM search_index
                WHERE content ~* @query
            ";

            if (entityType != null)
                sql += " AND entity_type = @entityType";

            sql += " ORDER BY id";

            await using var cmd = new NpgsqlCommand(sql, _conn);
            cmd.Parameters.AddWithValue("query", query);
            if (entityType != null)
                cmd.Parameters.AddWithValue("entityType", entityType);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new SearchIndexItem
                {
                    Id = reader.GetInt32(0),
                    EntityType = reader.GetString(1),
                    EntityId = reader.GetInt32(2),
                    Content = reader.GetString(3),
                });
            }

            return results;
        }



        public void Dispose()
        {
            _conn?.Dispose();
        }
    }
}
