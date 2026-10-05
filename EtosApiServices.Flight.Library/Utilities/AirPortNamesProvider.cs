using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EtosApiServices.Flight.Library.Utilities
{
    public sealed record AirportInfo(string Name, string City);

    public sealed class AirPortNamesProvider
    {
        private readonly IReadOnlyDictionary<string, AirportInfo> _map;

        public AirPortNamesProvider(string xmlFilePath)
        {
            if (string.IsNullOrWhiteSpace(xmlFilePath)) throw new ArgumentNullException(nameof(xmlFilePath));

            if (!File.Exists(xmlFilePath))
            {
                _map = new ReadOnlyDictionary<string, AirportInfo>(new Dictionary<string, AirportInfo>(StringComparer.OrdinalIgnoreCase));
                return;
            }

            try
            {
                var doc = XDocument.Load(xmlFilePath);
                var dict = doc.Root?
                    .Elements("AirPortNames")
                    .Select(e =>
                    {
                        var code = (e.Element("AirPortCode")?.Value ?? string.Empty).Trim();
                        var name = (e.Element("AirPortName")?.Value ?? string.Empty).Trim();
                        // correct element name is "ReferenceCity" in the XML
                        var city = (e.Element("ReferenceCity")?.Value ?? string.Empty).Trim();
                        return new { code, name, city };
                    })
                    .Where(x => !string.IsNullOrEmpty(x.code))
                    .GroupBy(x => x.code, StringComparer.OrdinalIgnoreCase)
                    // prefer an entry that has either a name or city, otherwise use first
                    .Select(g => g.FirstOrDefault(x => !string.IsNullOrEmpty(x.name) || !string.IsNullOrEmpty(x.city)) ?? g.First())
                    .ToDictionary(x => x.code, x => new AirportInfo(x.name, x.city), StringComparer.OrdinalIgnoreCase)
                    ?? new Dictionary<string, AirportInfo>(StringComparer.OrdinalIgnoreCase);

                _map = new ReadOnlyDictionary<string, AirportInfo>(dict);
            }
            catch
            {
                _map = new ReadOnlyDictionary<string, AirportInfo>(new Dictionary<string, AirportInfo>(StringComparer.OrdinalIgnoreCase));
            }
        }

        public bool TryGetAirport(string airportCode, out AirportInfo? airportInfo)
        {
            if (string.IsNullOrWhiteSpace(airportCode))
            {
                airportInfo = null;
                return false;
            }
            return _map.TryGetValue(airportCode.Trim(), out airportInfo);
        }

        // Backwards-compatible name lookup
        public bool TryGetName(string airportCode, out string? airportName)
        {
            if (TryGetAirport(airportCode, out var info) && info is not null)
            {
                airportName = info.Name;
                return true;
            }

            airportName = null;
            return false;
        }

        public bool TryGetCity(string airportCode, out string? city)
        {
            if (TryGetAirport(airportCode, out var info) && info is not null)
            {
                city = info.City;
                return true;
            }

            city = null;
            return false;
        }

        public string GetNameOrDefault(string airportCode, string? defaultValue = null)
        {
            if (TryGetName(airportCode, out var name))
                return name ?? defaultValue ?? airportCode ?? string.Empty;
            return defaultValue ?? airportCode ?? string.Empty;
        }

        public string GetCityOrDefault(string airportCode, string? defaultValue = null)
        {
            if (TryGetCity(airportCode, out var city))
                return city ?? defaultValue ?? airportCode ?? string.Empty;
            return defaultValue ?? airportCode ?? string.Empty;
        }

        // compatibility: returns code -> airport name
        public IReadOnlyDictionary<string, string> GetAll() =>
            new ReadOnlyDictionary<string, string>(_map.ToDictionary(kv => kv.Key, kv => kv.Value.Name, StringComparer.OrdinalIgnoreCase));

        // richer map: code -> AirportInfo (name + city)
        public IReadOnlyDictionary<string, AirportInfo> GetAllAirportInfo() => _map;
    }
}
