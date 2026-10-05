using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EtosApiServices.Flight.Library.Utilities
{
    public sealed record AirlineCityInfo(string CityName, string CountryCode, string AssociatedAirports, IReadOnlyList<string> AssociatedAirportCodes);

    public sealed class AirlineCitiesProvider
    {
        private readonly IReadOnlyDictionary<string, AirlineCityInfo> _byCityCode;
        private readonly IReadOnlyDictionary<string, AirlineCityInfo> _byAirportCode;

        public AirlineCitiesProvider(string xmlFilePath)
        {
            if (string.IsNullOrWhiteSpace(xmlFilePath)) throw new ArgumentNullException(nameof(xmlFilePath));

            if (!File.Exists(xmlFilePath))
            {
                _byCityCode = new ReadOnlyDictionary<string, AirlineCityInfo>(new Dictionary<string, AirlineCityInfo>(StringComparer.OrdinalIgnoreCase));
                _byAirportCode = new ReadOnlyDictionary<string, AirlineCityInfo>(new Dictionary<string, AirlineCityInfo>(StringComparer.OrdinalIgnoreCase));
                return;
            }

            try
            {
                var doc = XDocument.Load(xmlFilePath);
                var entries = doc.Root?
                    .Elements("AirLineCities")
                    .Select(e =>
                    {
                        var code = (e.Element("CityCode")?.Value ?? string.Empty).Trim();
                        var name = (e.Element("CityName")?.Value ?? string.Empty).Trim();
                        var country = (e.Element("CountryCode")?.Value ?? string.Empty).Trim();
                        var assoc = (e.Element("AssociatedAirports")?.Value ?? string.Empty).Trim();
                        var airports = assoc
                            .Split(new[] { ' ', '\t', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(a => a.Trim())
                            .Where(a => !string.IsNullOrEmpty(a))
                            .ToList();
                        return new { code, name, country, assoc, airports };
                    })
                    .Where(x => !string.IsNullOrEmpty(x.code))
                    .ToList();

                var byCity = new Dictionary<string, AirlineCityInfo>(StringComparer.OrdinalIgnoreCase);
                var byAirport = new Dictionary<string, AirlineCityInfo>(StringComparer.OrdinalIgnoreCase);

                foreach (var e in entries)
                {
                    var info = new AirlineCityInfo(e.name, e.country, e.assoc, e.airports.AsReadOnly());
                    if (!byCity.ContainsKey(e.code))
                        byCity[e.code] = info;

                    foreach (var ap in e.airports)
                    {
                        if (!byAirport.ContainsKey(ap))
                            byAirport[ap] = info;
                    }
                }

                _byCityCode = new ReadOnlyDictionary<string, AirlineCityInfo>(byCity);
                _byAirportCode = new ReadOnlyDictionary<string, AirlineCityInfo>(byAirport);
            }
            catch
            {
                _byCityCode = new ReadOnlyDictionary<string, AirlineCityInfo>(new Dictionary<string, AirlineCityInfo>(StringComparer.OrdinalIgnoreCase));
                _byAirportCode = new ReadOnlyDictionary<string, AirlineCityInfo>(new Dictionary<string, AirlineCityInfo>(StringComparer.OrdinalIgnoreCase));
            }
        }

        public bool TryGetByCityCode(string cityCode, out AirlineCityInfo? cityInfo)
        {
            if (string.IsNullOrWhiteSpace(cityCode)) { cityInfo = null; return false; }
            return _byCityCode.TryGetValue(cityCode.Trim(), out cityInfo);
        }

        public bool TryGetByAirportCode(string airportCode, out AirlineCityInfo? cityInfo)
        {
            if (string.IsNullOrWhiteSpace(airportCode)) { cityInfo = null; return false; }
            return _byAirportCode.TryGetValue(airportCode.Trim(), out cityInfo);
        }

        public string GetCityNameByAirportCodeOrDefault(string airportCode, string? defaultValue = null)
        {
            if (TryGetByAirportCode(airportCode, out var info) && info is not null && !string.IsNullOrEmpty(info.CityName))
                return info.CityName;
            return defaultValue ?? airportCode ?? string.Empty;
        }

        public string GetCityNameByCityCodeOrDefault(string cityCode, string? defaultValue = null)
        {
            if (TryGetByCityCode(cityCode, out var info) && info is not null && !string.IsNullOrEmpty(info.CityName))
                return info.CityName;
            return defaultValue ?? cityCode ?? string.Empty;
        }

        public IReadOnlyDictionary<string, AirlineCityInfo> GetAllByCityCode() => _byCityCode;
        public IReadOnlyDictionary<string, AirlineCityInfo> GetAllByAirportCode() => _byAirportCode;
    }
}
