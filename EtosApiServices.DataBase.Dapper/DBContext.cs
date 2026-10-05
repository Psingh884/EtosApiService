using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Data;
using EtosApiServices.Model.Setting;

namespace EtosApiServices.DataBase.Dapper
{
    public class DBContext
    {
        private readonly IConfiguration _configuration;
        readonly ConnectionStrings _connectionStrings;

        public DBContext(IConfiguration configuration, IOptions<ConnectionStrings> connectionStrings)
        {
            _configuration = configuration;
            _connectionStrings = connectionStrings?.Value ?? throw new ArgumentException(nameof(connectionStrings));
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            return await Task.FromResult<IDbConnection>(new SqlConnection(_connectionStrings.APIDBConnectionString));
        }

        public async Task<IDbConnection> CreateConnectionAsync(string DBConnectionString)
        {
            return await Task.FromResult<IDbConnection>(new SqlConnection(string.IsNullOrEmpty(DBConnectionString) ? _connectionStrings.APIDBConnectionString : DBConnectionString));
        }
    }
}
