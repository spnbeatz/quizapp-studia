using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Models;

namespace projekt.Interfaces
{
    public interface ISearchService
    {
        Task<List<SearchIndexItem>> SearchAsync(string query, string? entityType = null);
    }
}
