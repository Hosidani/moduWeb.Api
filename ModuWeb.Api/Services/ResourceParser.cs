using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models;

namespace ModuWeb.Api.Services
{
    public static class ResourceParser
    {
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
    }
}
