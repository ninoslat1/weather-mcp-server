using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherMCP.Extensions;

namespace WeatherMCP.Tools
{
    public static class WeatherTools
    {
        [McpServerTool, Description("Get weather alerts for INA state")]
        public static async Task<string> GetAlerts(
            HttpClient httpClient,
            [Description("The INA state to get alerts for")] string state
        )
        {
            using var jsonDoc = await httpClient.ReadJsonDocAsync($"/alerts/active/area/{state}");
            var jsonElement = jsonDoc.RootElement;
            var alerts = jsonElement.GetProperty("features").EnumerateArray();

            if (!alerts.Any())
            {
                return $"No active alerts for this state.";
            }

            return string.Join("\n--\n", alerts.Select(alert =>
            {
                JsonElement properties = alert.GetProperty("properties");
                return $"""
                    Event: {properties.GetProperty("event").GetString()}
                    Area: {properties.GetProperty("areaDesc").GetString()}
                    Severity: {properties.GetProperty("severity").GetString()}
                    Description: {properties.GetProperty("description").GetString()}
                    Instruction: {properties.GetProperty("instruction").GetString()}
                """;
            }));
        }

        [McpServerTool, Description("Get weather forecast for a location in INA")]
        public static async Task<string> GetForecast(
            HttpClient httpClient,
            [Description("The latitude of the location")] double latitude,
            [Description("The longitude of the location")] double longitude
        )
        {
            var pointUri = string.Create(CultureInfo.InvariantCulture, $"/points/{latitude},{longitude}");
            using var jsonDoc = await httpClient.ReadJsonDocAsync(pointUri);
            var forecastUri = jsonDoc.RootElement.GetProperty("properties").GetProperty("forecast").GetString() ?? throw new Exception($"No forecast url provided by {httpClient.BaseAddress}/points/{latitude},{longitude}");

            using var forecastDoc = await httpClient.ReadJsonDocAsync(forecastUri);
            var periods = forecastDoc.RootElement.GetProperty("properties").GetProperty("periods").EnumerateArray();

            return string.Join("\n---\n", periods.Select(period => $"""
                {period.GetProperty("name").GetString()}
                Temperature: {period.GetProperty("temperature").GetInt32()}°F
                Wind: {period.GetProperty("windSpeed").GetString()} {period.GetProperty("windDirection").GetString()}
                Forecast: {period.GetProperty("detailedForecast").GetString()}
                """));
        }
    }
}
