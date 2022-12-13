using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PingWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {

        [HttpGet]
        public PingResult Get()
        {
            var feature = HttpContext.Features.Get<IHttpConnectionFeature>();
            var ip = feature?.LocalIpAddress?.ToString();

            var osNameAndVersion = "";
            string assemblyFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            osNameAndVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription.ToString();

            var result = new PingResult
            {
                Host = osNameAndVersion,
                HostName = Environment.MachineName,
                HostIp = ip,
                SystemTime = DateTime.UtcNow.ToString(),
                Version = assemblyFileVersion
            };

            return result;
        }
    }

    public struct PingResult
    {
        public string HostName { get; internal set; }
        public string HostIp { get; internal set; }
        public string SystemTime { get; internal set; }
        public string Host { get; internal set; }
        public string Version { get; internal set; }
    }
}
