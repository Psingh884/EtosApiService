using Dapper;
using EtosApiServices.Data.Library.Common;
using EtosApiServices.GenericRepository.Contracts;
using EtosApiServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Data.Library.Repository
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        readonly IGenericRepository _genericRepository;

        public AuthenticationRepository(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository ?? throw new ArgumentNullException(nameof(genericRepository));
        }

        public async Task<CompanyUser> ValidateCompanyUserAsync(string username, string AppKey)
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserName", username);
            parameters.Add("AppKey", AppKey);
            var ValidationResult = await _genericRepository.ExecuteQueryObjectAsync<CompanyUser>("Proc_APIGetLoginInfo", parameters);
            if (ValidationResult == null || ValidationResult.PlayerId <= 0)
            {
                throw new UnauthorizedAccessException("Invalid credentials provided.");
            }
            return ValidationResult;
        }

        public async Task<string> GetCompanyDBConnectionString(int PlayerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("PlayerId", PlayerId);
            var ValidationResult = await _genericRepository.ExecuteQueryObjectAsync<CompanyDBSetup>("Proc_APIGetDBConnectionString", parameters);
            if (ValidationResult == null || ValidationResult.DBConnectionId <= 0)
            {
                throw new Exception("DB Connection setup is not done.");
            }
            string connectionString = $"Server={ValidationResult.ServerName};Initial Catalog={ValidationResult.InitialCatalog};User Id={ValidationResult.DBUserId};password={ValidationResult.DBpassword};Encrypt=True;TrustServerCertificate=True;";

            return connectionString;
        }
    }
}
