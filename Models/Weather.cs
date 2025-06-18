using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMCP.Models
{
    [McpServerToolType]
    public static class Weather
    {
        [McpServerTool, Description("Get weather alerts for INA state")]
        public static async Task<string> GetAlerts()
        {

        }
    }
}
