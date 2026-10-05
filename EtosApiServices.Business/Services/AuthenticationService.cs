using EtosApiServices.Business.Contract;
using EtosApiServices.Data.Library.Common;
using EtosApiServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Business.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        readonly IAuthenticationRepository _authenticationRepository;
        public AuthenticationService(IAuthenticationRepository authenticationRepository)
        {
            this._authenticationRepository = authenticationRepository;
        }

        public async Task<CompanyUser> ValidateCompanyUserAsync(string UserName, string Password, string AppKey)
        {
            var ValidationResult = await _authenticationRepository.ValidateCompanyUserAsync(UserName, AppKey);

            //Encryption encryption = new Encryption(HashingAlgoEnum.SHA256);
            //if (!await encryption.ValidateHashAsync(Password, ValidationResult.Password ?? "", ""))
            //{
            //    throw new Exception("Invalid credentials provided.");
            //}
            ValidationResult.DBConnectionString = await _authenticationRepository.GetCompanyDBConnectionString(ValidationResult.PlayerId);
            return ValidationResult;

        }

    }
}
