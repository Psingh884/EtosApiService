using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EtosApiServices.Flight.Library.Utilities
{
    public sealed class AirlineNamesProvider
    {
        private readonly IReadOnlyDictionary<string, string> _map;

        public AirlineNamesProvider(string xmlFilePath)
        {
            if (string.IsNullOrWhiteSpace(xmlFilePath)) throw new ArgumentNullException(nameof(xmlFilePath));

            if (!File.Exists(xmlFilePath))
            {
                _map = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                return;
            }

            try
            {
                var doc = XDocument.Load(xmlFilePath);
                var dict = doc.Root?
                    .Elements("AirLineNames")
                    .Select(e =>
                    {
                        var code = (e.Element("AirlineCode")?.Value ?? string.Empty).Trim();
                        var name = (e.Element("AirlineName")?.Value ?? e.Element("AirlineNameDescription")?.Value ?? string.Empty).Trim();
                        return new { code, name };
                    })
                    .Where(x => !string.IsNullOrEmpty(x.code))
                    .ToDictionary(x => x.code, x => x.name, StringComparer.OrdinalIgnoreCase)
                    ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                _map = new ReadOnlyDictionary<string, string>(dict);
            }
            catch
            {
                _map = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            }
        }

        public bool TryGetName(string airlineCode, out string? airlineName)
        {
            if (string.IsNullOrWhiteSpace(airlineCode))
            {
                airlineName = null;
                return false;
            }
            return _map.TryGetValue(airlineCode.Trim(), out airlineName);
        }

        public string GetNameOrDefault(string airlineCode, string? defaultValue = null)
        {
            return TryGetName(airlineCode, out var name) ? name ?? defaultValue ?? airlineCode : defaultValue ?? airlineCode;
        }

        public IReadOnlyDictionary<string, string> GetAll() => _map;
    }
}
