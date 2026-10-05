using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EtosApiServices.MongoDb.Library
{
    public static class AppConfig
    {
        private static readonly Lazy<IConfiguration> _configuration = new Lazy<IConfiguration>(() =>
        {
            var basePath = AppContext.BaseDirectory;

            if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
            {
                basePath = Directory.GetCurrentDirectory();
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)                
                .AddEnvironmentVariables();

            return builder.Build();
        });

        public static IConfiguration Configuration => _configuration.Value;

        public static string GetConfigValue(string key)
        {
            return Configuration[key] ?? "";
        }

        public static string GetDBConnectionString(string name = "DefaultConnection")
        {
            return Configuration.GetConnectionString(name)
                ?? throw new InvalidOperationException($"Connection string '{name}' not found.");
        }

        public static string GetMSSQLDBConnectionString(string key = "DataBase:SQL:MSSQL:MSSQL_Connection")
        {
            return Configuration[key] ?? throw new KeyNotFoundException($"Key '{key}' not found in appsettings.json.");
        }

        public static string GetMongoDBConnectionString(string key = "DataBase:NoSQL:MongoDB:MongoDB_Connection")
        {
            return Configuration[key] ?? throw new KeyNotFoundException($"Key '{key}' not found in appsettings.json.");
        }

        public static string ServerMapPath(string relativePath)
        {
            var basePath = GetConfigValue("Paths:FileServerPath") ?? throw new Exception("FileServerPath not configured");
            return Path.Combine(basePath, relativePath.TrimStart('~', '/', '\\').Replace('/', Path.DirectorySeparatorChar));
        }

        public static void CreateDirectoryServerMapPath(string relativePath)
        {
            try
            {
                var basePath = GetConfigValue("Paths:FileServerPath") ?? throw new Exception("FileServerPath not configured");
                string folderPath = Path.Combine(basePath, relativePath.TrimStart('~', '/', '\\').Replace('/', Path.DirectorySeparatorChar));
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static List<List<T>> GetSetOfHotelIds<T>(List<T> SHotelIDs, int RowCount)
        {
            var Result = new List<List<T>>();

            try
            {
                if (SHotelIDs != null && SHotelIDs.Count != 0)
                {
                    SHotelIDs = SHotelIDs.Select(b => b).Distinct().ToList();
                    if (SHotelIDs != null && SHotelIDs.Count != 0)
                    {
                        int Skip = 0;
                        int Rcount = 1;
                        if (SHotelIDs.Count >= RowCount)
                        {
                            Rcount = SHotelIDs.Count / RowCount;
                        }
                        if (SHotelIDs.Count >= RowCount)
                        {
                            var mode = SHotelIDs.Count % RowCount;
                            if (mode != 0)
                            {
                                Rcount = Rcount + 1;
                            }
                        }
                        int Take = RowCount;
                        for (int i = 1; i <= Rcount; i++)
                        {
                            var Rowdata = SHotelIDs.Skip(Skip).Take(Take).ToList();
                            Result.Add(Rowdata);
                            Skip = Skip + Take;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Errorlog.InsertLogError(ex, 0);
            }
            return Result;

        }
    }
}
