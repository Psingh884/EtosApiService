using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EtosApiServices.Model.Filter
{
    public class SwaggerExcludeSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema == null || context == null || context.Type == null) return;

            // If the whole type is annotated, clear the schema so it doesn't expose properties.
            if (context.Type.GetCustomAttributes(true).OfType<SwaggerExcludeAttribute>().Any())
            {
                schema.Properties?.Clear();
                schema.Type = "object";
                schema.Description = "Excluded from Swagger";
                return;
            }

            // Remove properties annotated with [SwaggerExclude]
            var excludedProperties = context.Type
                                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                            .Where(p => p.GetCustomAttributes(true).OfType<SwaggerExcludeAttribute>().Any())
                                            .ToArray();

            if (!excludedProperties.Any() || schema.Properties == null) return;

            foreach (var prop in excludedProperties)
            {
                // prefer explicit JsonPropertyName if present; otherwise try camel-cased name and original name
                var jsonName = prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                               ?? char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1);

                if (schema.Properties.ContainsKey(jsonName))
                    schema.Properties.Remove(jsonName);

                if (schema.Properties.ContainsKey(prop.Name))
                    schema.Properties.Remove(prop.Name);
            }
        }
    }
}
