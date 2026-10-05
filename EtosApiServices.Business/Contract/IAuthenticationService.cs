using EtosApiServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Business.Contract
{
    public interface IAuthenticationService
    {
        Task<CompanyUser> ValidateCompanyUserAsync(string username, string Password, string AppKey);
    }
}
