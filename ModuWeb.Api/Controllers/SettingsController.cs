using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModuWeb.Api.Services;
using ModuWeb.Api.Models.ResourceSystem.ResourceDtos;

namespace ModuWeb.Api.Controllers
{
    [Route("api/settings")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ILogger<SettingsController> _logger;
        private readonly IConversionService _conversionService;
        private readonly string _settingsStoragePath;

        public SettingsController(ILogger<SettingsController> logger, IConversionService conversionService, IWebHostEnvironment environment)
        {
            _logger = logger;
            _conversionService = conversionService;
            
            // Create a dedicated folder for settings files
            _settingsStoragePath = Path.Combine(environment.ContentRootPath, "SettingsFiles");
            
            // Ensure the directory exists
            if (!Directory.Exists(_settingsStoragePath))
            {
                Directory.CreateDirectory(_settingsStoragePath);
            }
        }

        // CREATE/UPDATE - POST: api/settings
        [HttpPost]
        public async Task<IActionResult> SaveUserConfiguration([FromBody] UserConfigurationDto request)
        {
            try
            {
                _logger.LogInformation("Saving configuration '{ConfigName}' for user: {UserName}", request.ConfigurationName, request.UserName);

                // Generate user's YAML file path
                var fileName = $"{request.UserName}_configurations.yaml";
                var filePath = Path.Combine(_settingsStoragePath, fileName);

                // Load existing configurations or create new structure
                var userConfigurations = new Dictionary<string, object>();
                
                if (System.IO.File.Exists(filePath))
                {
                    var existingYaml = await System.IO.File.ReadAllTextAsync(filePath);
                    var existingJson = _conversionService.ConvertYamlToJson(existingYaml);
                    userConfigurations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(existingJson) ?? new();
                }

                // Add/update the specific configuration
                var configData = System.Text.Json.JsonSerializer.Deserialize<object>(request.ConfigurationData);
                userConfigurations[request.ConfigurationName] = new
                {
                    data = configData,
                    lastUpdated = DateTime.UtcNow,
                    resourceName = request.ConfigurationName
                };

                // Convert back to YAML and save
                var updatedJson = System.Text.Json.JsonSerializer.Serialize(userConfigurations, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                var yamlContent = _conversionService.ConvertJsonToYaml(updatedJson);
                await System.IO.File.WriteAllTextAsync(filePath, yamlContent);

                _logger.LogInformation("Configuration saved successfully: {FileName}", fileName);

                return Ok(new { 
                    message = "Configuration saved successfully", 
                    userId = request.UserId,
                    configurationName = request.ConfigurationName,
                    fileName = fileName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving configuration '{ConfigName}' for user: {UserName}", request.ConfigurationName, request.UserName);
                return StatusCode(500, new { message = "Failed to save configuration", error = ex.Message });
            }
        }

        // READ - GET: api/settings/{userName}
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetUserConfigurations(string userName)
        {
            try
            {
                _logger.LogInformation("Getting configurations for user: {UserName}", userName);

                var fileName = $"{userName}_configurations.yaml";
                var filePath = Path.Combine(_settingsStoragePath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return Ok(new { configurations = new Dictionary<string, object>() });
                }

                var yamlContent = await System.IO.File.ReadAllTextAsync(filePath);
                var jsonContent = _conversionService.ConvertYamlToJson(yamlContent);
                var configurations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);

                return Ok(new { configurations });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting configurations for user: {UserName}", userName);
                return StatusCode(500, new { message = "Failed to get configurations", error = ex.Message });
            }
        }

        // GET CONFIG LIST - GET: api/settings/{userName}/list
        [HttpGet("{userName}/list")]
        public async Task<IActionResult> GetConfigurationsList(string userName)
        {
            try
            {
                _logger.LogInformation("Getting configurations list for user: {UserName}", userName);

                var fileName = $"{userName}_configurations.yaml";
                var filePath = Path.Combine(_settingsStoragePath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return Ok(new { configurations = new string[0] });
                }

                var yamlContent = await System.IO.File.ReadAllTextAsync(filePath);
                var jsonContent = _conversionService.ConvertYamlToJson(yamlContent);
                var configurations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);

                // Return only configuration names and metadata (not the full data)
                var configList = configurations?.Keys.Select(key => new
                {
                    name = key,
                    lastUpdated = GetLastUpdatedFromConfig(configurations[key])
                }).ToArray() ?? new object[0];

                return Ok(new { configurations = configList });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting configurations list for user: {UserName}", userName);
                return StatusCode(500, new { message = "Failed to get configurations list", error = ex.Message });
            }
        }

        private DateTime GetLastUpdatedFromConfig(object config)
        {
            try
            {
                var configJson = System.Text.Json.JsonSerializer.Serialize(config);
                var configObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(configJson);
                if (configObj?.ContainsKey("lastUpdated") == true)
                {
                    if (DateTime.TryParse(configObj["lastUpdated"].ToString(), out var date))
                        return date;
                }
            }
            catch { }
            return DateTime.MinValue;
        }
        [HttpGet("{userName}/{configName}")]
        public async Task<IActionResult> GetSpecificConfiguration(string userName, string configName)
        {
            try
            {
                _logger.LogInformation("Getting configuration '{ConfigName}' for user: {UserName}", configName, userName);

                var fileName = $"{userName}_configurations.yaml";
                var filePath = Path.Combine(_settingsStoragePath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "Configuration not found" });
                }

                var yamlContent = await System.IO.File.ReadAllTextAsync(filePath);
                var jsonContent = _conversionService.ConvertYamlToJson(yamlContent);
                var configurations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);

                if (!configurations.ContainsKey(configName))
                {
                    return NotFound(new { message = $"Configuration '{configName}' not found" });
                }

                return Ok(new { 
                    configurationName = configName,
                    configuration = configurations[configName] 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting configuration '{ConfigName}' for user: {UserName}", configName, userName);
                return StatusCode(500, new { message = "Failed to get configuration", error = ex.Message });
            }
        }

        // UPDATE - PUT: api/settings/{userName}/{configName}
        [HttpPut("{userName}/{configName}")]
        public async Task<IActionResult> UpdateConfiguration(string userName, string configName, [FromBody] ConfigurationUpdateDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating configuration '{ConfigName}' for user: {UserName}", configName, userName);

                var fileName = $"{userName}_configurations.yaml";
                var filePath = Path.Combine(_settingsStoragePath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "User configurations not found" });
                }

                var yamlContent = await System.IO.File.ReadAllTextAsync(filePath);
                var jsonContent = _conversionService.ConvertYamlToJson(yamlContent);
                var configurations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent) ?? new();

                if (!configurations.ContainsKey(configName))
                {
                    return NotFound(new { message = $"Configuration '{configName}' not found" });
                }

                // Update the specific configuration
                var configData = System.Text.Json.JsonSerializer.Deserialize<object>(updateDto.ConfigurationData);
                configurations[configName] = new
                {
                    data = configData,
                    lastUpdated = DateTime.UtcNow,
                    resourceName = configName
                };

                // Save back to YAML
                var updatedJson = System.Text.Json.JsonSerializer.Serialize(configurations, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                var updatedYaml = _conversionService.ConvertJsonToYaml(updatedJson);
                await System.IO.File.WriteAllTextAsync(filePath, updatedYaml);

                _logger.LogInformation("Configuration updated successfully: {ConfigName}", configName);

                return Ok(new { message = "Configuration updated successfully", configurationName = configName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating configuration '{ConfigName}' for user: {UserName}", configName, userName);
                return StatusCode(500, new { message = "Failed to update configuration", error = ex.Message });
            }
        }

        // DELETE - DELETE: api/settings/{userName}/{configName}
        [HttpDelete("{userName}/{configName}")]
        public async Task<IActionResult> DeleteConfiguration(string userName, string configName)
        {
            try
            {
                _logger.LogInformation("Deleting configuration '{ConfigName}' for user: {UserName}", configName, userName);

                var fileName = $"{userName}_configurations.yaml";
                var filePath = Path.Combine(_settingsStoragePath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "User configurations not found" });
                }

                var yamlContent = await System.IO.File.ReadAllTextAsync(filePath);
                var jsonContent = _conversionService.ConvertYamlToJson(yamlContent);
                var configurations = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent) ?? new();

                if (!configurations.ContainsKey(configName))
                {
                    return NotFound(new { message = $"Configuration '{configName}' not found" });
                }

                // Remove the configuration
                configurations.Remove(configName);

                // Save back to YAML
                var updatedJson = System.Text.Json.JsonSerializer.Serialize(configurations, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                var updatedYaml = _conversionService.ConvertJsonToYaml(updatedJson);
                await System.IO.File.WriteAllTextAsync(filePath, updatedYaml);

                _logger.LogInformation("Configuration deleted successfully: {ConfigName}", configName);

                return Ok(new { message = "Configuration deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting configuration '{ConfigName}' for user: {UserName}", configName, userName);
                return StatusCode(500, new { message = "Failed to delete configuration", error = ex.Message });
            }
        }
    }

    // Simple DTOs for the new structure
    public class UserConfigurationDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ConfigurationName { get; set; } = string.Empty; // The name/key for this config
        public string ConfigurationData { get; set; } = string.Empty; // JSON data
    }

    public class ConfigurationUpdateDto
    {
        public string ConfigurationData { get; set; } = string.Empty;
    }
}