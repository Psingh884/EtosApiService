using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.GenericRepository.Contracts
{
    public interface IGenericRepository
    {
        Task<int> ExecuteNonQueryAsync(string Query, object? parameters = null, string? DBConnectionString = null);
        Task<IEnumerable<T>> ExecuteQueryListAsync<T>(string Query, object? parameters = null, string? DBConnectionString = null);
        Task<TEntity> ExecuteQueryObjectAsync<TEntity>(string Query, object? parameters = null, string? DBConnectionString = null);
        Task<(IEnumerable<T1> Result1, IEnumerable<T2> Result2)> ExecuteQueryMultipleAsync<T1, T2>(string Query, object? parameters = null, string? DBConnectionString = null);
        Task<T> ExecuteQueryAsync<T>(string v, DynamicParameters parameters, object value);
    }
}
