using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.MongoDb.Library
{
    public class MongoDbSetting
    {
        public string connectionString = string.Empty;// "mongodb://192.168.1.38:27017";
        public static string DataBaseName = string.Empty;// "eTos_DMC_ServiceContent";

        public MongoDbSetting()
        {
            connectionString = AppConfig.GetMongoDBConnectionString();
            DataBaseName = AppConfig.GetConfigValue("DataBase:NoSQL:MongoDB:MongoDataBase");
        }
    }
}
