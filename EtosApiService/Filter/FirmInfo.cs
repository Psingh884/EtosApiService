namespace EtosApiService.Filter
{
    public class FirmInfo
    {
        public const string AuthTokenHeader = "AuthToken";
        public const string AppKey = "AppKey";
        public const string UserName = "UserName";
        public const string Password = "Password";
        public const string ComapnyIdHeader = "ComapnyId";
        public const string PlayerIdHeader = "PlayerId";
        public const string CompanyUrlHeader = "CompanyUrl";
        public const string CompanyCurrencyHeader = "CompanyCurrency";
        public const string DBConnectionStringHeader = "DBConnectionString";

        public static string? GetStringHeader(HttpRequest request, string HeaderName)
        {
            return request.Headers.TryGetValue(HeaderName, out var Values) ? Values.ToString() : null;
        }
        public static int? GetIntHeader(HttpRequest request, string HeaderName)
        {
            if (request.Headers.TryGetValue(HeaderName, out var Values) && int.TryParse(Values.ToString(), out int result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
        public static int ComapnyId(HttpRequest request) => GetIntHeader(request, ComapnyIdHeader) ?? 0;
        public static int PlayerId(HttpRequest request) => GetIntHeader(request, PlayerIdHeader) ?? 0;
        public static string DBConnectionString(HttpRequest request) => GetStringHeader(request, DBConnectionStringHeader) ?? "";
        public static string CompanyUrl(HttpRequest request) => GetStringHeader(request, CompanyUrlHeader) ?? "";
        public static string CompanyCurrency(HttpRequest request) => GetStringHeader(request, CompanyCurrencyHeader) ?? "";
    }
}
