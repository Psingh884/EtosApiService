using Dapper;
using EtosApiServices.DataBase.Dapper;
using EtosApiServices.GenericRepository.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.GenericRepository.Repository
{
    public class GenericRepository : IGenericRepository
    {
        private readonly DBContext _dBContext;
        private const int commandTimeoutSeconds = 0;

        public GenericRepository(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<int> ExecuteNonQueryAsync(string Query, object? parameters = null, string? DBConnectionString = null)
        {
            using (var connection = await _dBContext.CreateConnectionAsync(DBConnectionString ?? ""))
            {
                var command = new CommandDefinition(Query, parameters, commandType: CommandType.StoredProcedure, commandTimeout: commandTimeoutSeconds);
                var result = await connection.ExecuteAsync(command);
                return result == -1 ? 1 : result;
            }
        }
        public async Task<IEnumerable<T>> ExecuteQueryListAsync<T>(string Query, object? parameters = null, string? DBConnectionString = null)
        {
            using (var connection = await _dBContext.CreateConnectionAsync(DBConnectionString ?? ""))
            {
                var command = new CommandDefinition(Query, parameters, commandType: CommandType.StoredProcedure, commandTimeout: commandTimeoutSeconds);
                var result = await connection.QueryAsync<T>(command);
                return result;
            }
        }
        public async Task<TEntity> ExecuteQueryObjectAsync<TEntity>(string Query, object? parameters = null, string? DBConnectionString = null)
        {
            using (var connection = await _dBContext.CreateConnectionAsync(DBConnectionString ?? ""))
            {
                var command = new CommandDefinition(Query, parameters, commandType: CommandType.StoredProcedure, commandTimeout: commandTimeoutSeconds);
                var result = await connection.QueryFirstOrDefaultAsync<TEntity>(command);
                return result!;
            }
        }
        public async Task<(IEnumerable<T1> Result1, IEnumerable<T2> Result2)> ExecuteQueryMultipleAsync<T1, T2>(string Query, object? parameters = null, string? DBConnectionString = null)
        {
            using (var connection = await _dBContext.CreateConnectionAsync(DBConnectionString ?? ""))
            {
                var command = new CommandDefinition(Query, parameters, commandType: CommandType.StoredProcedure, commandTimeout: commandTimeoutSeconds);
                using var multi = await connection.QueryMultipleAsync(command);
                var result1 = await multi.ReadAsync<T1>();
                var result2 = await multi.ReadAsync<T2>();
                return (result1, result2);
            }
        }
        public async Task<T> ExecuteQueryAsync<T>(string Query, DynamicParameters parameters, object value)
        {
            using (var connection = await _dBContext.CreateConnectionAsync(value?.ToString() ?? ""))
            {
                var command = new CommandDefinition(Query, parameters, commandType: CommandType.StoredProcedure, commandTimeout: commandTimeoutSeconds);
                var result = await connection.QueryFirstOrDefaultAsync<T>(command);
                return result!;
            }
        }
    }
}
