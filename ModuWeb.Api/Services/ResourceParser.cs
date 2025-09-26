using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models;

namespace ModuWeb.Api.Services
{
    public static class ResourceParser
    {
        // Parse full object
        public static async Task<T?> ParseAsync<T>(ApplicationDbContext db, Guid uuid)
        {
            var resource = await db.Resources.FirstOrDefaultAsync(r => r.Id == uuid);
            if (resource == null || string.IsNullOrEmpty(resource.Content))
                return default;

            return JsonSerializer.Deserialize<T>(resource.Content);
        }

        // Parse single element by name
        public static async Task<T?> ParseElementByNameAsync<T>(ApplicationDbContext db, Guid uuid, string elementName)
        {
            var resource = await db.Resources.FirstOrDefaultAsync(r => r.Id == uuid);
            if (resource == null || string.IsNullOrEmpty(resource.Content))
                return default;

            using var doc = JsonDocument.Parse(resource.Content);
            if (doc.RootElement.TryGetProperty(elementName, out var element))
            {
                return element.Deserialize<T>();
            }
            return default;
        }

        // Parse multiple elements by name
        public static async Task<Dictionary<string, object?>> ParseElementsAsync(ApplicationDbContext db, Guid uuid, IEnumerable<string> elementNames)
        {
            var result = new Dictionary<string, object?>();

            var resource = await db.Resources.FirstOrDefaultAsync(r => r.Id == uuid);
            if (resource == null || string.IsNullOrEmpty(resource.Content))
                return result;

            using var doc = JsonDocument.Parse(resource.Content);
            foreach (var name in elementNames)
            {
                if (doc.RootElement.TryGetProperty(name, out var element))
                {
                    result[name] = element.Deserialize<object>();
                }
            }

            return result;
        }
    }
}
