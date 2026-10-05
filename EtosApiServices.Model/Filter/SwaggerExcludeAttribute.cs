using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Model.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class SwaggerExcludeAttribute : Attribute
    {
    }
}
