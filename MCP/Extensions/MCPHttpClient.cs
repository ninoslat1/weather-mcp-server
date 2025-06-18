using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherMCP.MCP.Extensions
{
    internal static class MCPHttpClient
    {
        public static async Task<JsonDocument> ReadJsonDoc(this HttpClient httpClient, string reqUri)
        {
            using var response = await httpClient.GetAsync(reqUri);
            response.EnsureSuccessStatusCode();
            return await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        }
    }
}
