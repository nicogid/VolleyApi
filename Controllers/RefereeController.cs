using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VolleyApi.Model;

namespace VolleyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RefereeController : ControllerBase
    {
        private readonly ILogger<RefereeController> _logger;
        private readonly string url = "https://www.ffvbbeach.org/ffvbapp/webcca/lect_acces_first.php";

        public RefereeController(ILogger<RefereeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("test")]
        public Referee Get()
        {
            return new Referee
            {
                UserId = "2*****6",
                Password = "******"
            };
        }

        [HttpPost("GetReferee")]
        public async Task<IEnumerable<string>> GetReferee()
        {
            using var client = new HttpClient();
            try
            {

                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("ws_cnclub", "2*****6");
                data.Add("ws_passwd", "*******");
                HttpContent formContent = new FormUrlEncodedContent(data);
                var result = await client.PostAsync(url, formContent);

                IEnumerable<string> cookies = result.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

                return cookies;

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return null;
        }
    }
}
