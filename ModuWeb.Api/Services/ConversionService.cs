using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ModuWeb.Api.Services
{
    public class ConversionService : IConversionService
    {
        private readonly ILogger<ConversionService> _logger;
        private readonly ISerializer _yamlSerializer;
        private readonly IDeserializer _yamlDeserializer;

        public ConversionService(ILogger<ConversionService> logger)
        {
            _logger = logger;
            
            // Configure YAML serializer with camelCase naming convention
            _yamlSerializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            // Configure YAML deserializer with camelCase naming convention
            _yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
        }

        public string ConvertJsonToYaml(string jsonContent)
        {
            try
            {
                _logger.LogInformation("Converting JSON to YAML");

                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    throw new ArgumentException("JSON content cannot be null or empty", nameof(jsonContent));
                }

                // Parse JSON into a JsonDocument to properly handle the structure
                using var jsonDocument = JsonDocument.Parse(jsonContent);
                var jsonObject = ConvertJsonElementToObject(jsonDocument.RootElement);

                // Convert object to YAML
                var yamlContent = _yamlSerializer.Serialize(jsonObject);

                _logger.LogInformation("Successfully converted JSON to YAML");
                return yamlContent;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid JSON format provided for conversion to YAML");
                throw new ArgumentException("Invalid JSON format", nameof(jsonContent), ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting JSON to YAML");
                throw new InvalidOperationException("Failed to convert JSON to YAML", ex);
            }
        }

        public string ConvertYamlToJson(string yamlContent)
        {
            try
            {
                _logger.LogInformation("Converting YAML to JSON");

                if (string.IsNullOrWhiteSpace(yamlContent))
                {
                    throw new ArgumentException("YAML content cannot be null or empty", nameof(yamlContent));
                }

                // Parse YAML into an object
                var yamlObject = _yamlDeserializer.Deserialize<object>(yamlContent);

                // Convert object to JSON with pretty printing
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var jsonContent = JsonSerializer.Serialize(yamlObject, options);

                _logger.LogInformation("Successfully converted YAML to JSON");
                return jsonContent;
            }
            catch (YamlDotNet.Core.YamlException ex)
            {
                _logger.LogError(ex, "Invalid YAML format provided for conversion to JSON");
                throw new ArgumentException("Invalid YAML format", nameof(yamlContent), ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting YAML to JSON");
                throw new InvalidOperationException("Failed to convert YAML to JSON", ex);
            }
        }

        private object ConvertJsonElementToObject(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var dict = new Dictionary<string, object>();
                    foreach (var property in element.EnumerateObject())
                    {
                        dict[property.Name] = ConvertJsonElementToObject(property.Value);
                    }
                    return dict;

                case JsonValueKind.Array:
                    var list = new List<object>();
                    foreach (var item in element.EnumerateArray())
                    {
                        list.Add(ConvertJsonElementToObject(item));
                    }
                    return list;

                case JsonValueKind.String:
                    return element.GetString()!;

                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var longValue))
                        return longValue;
                    return element.GetDouble();

                case JsonValueKind.True:
                    return true;

                case JsonValueKind.False:
                    return false;

                case JsonValueKind.Null:
                    return null!;

                default:
                    return element.ToString();
            }
        }
    }
}