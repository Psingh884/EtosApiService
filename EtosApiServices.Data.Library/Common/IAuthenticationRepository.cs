using EtosApiServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Data.Library.Common
{
    public interface IAuthenticationRepository
    {
        Task<CompanyUser> ValidateCompanyUserAsync(string username, string AppKey);

        Task<string> GetCompanyDBConnectionString(int PlayerId);
    }
}
