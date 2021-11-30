using HtmlAgilityPack;
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
        private readonly string urlConnexion = "https://www.ffvbbeach.org/ffvbapp/webcca/lect_acces_first.php";
        private readonly string urlDesignation = "https://www.ffvbbeach.org/ffvbapp/webcca/cca_recap_aff.php?PHPSESSID=j94s0q2iladfs29tcb4tnt2vm1";
        Dictionary<string, string> data = new Dictionary<string, string>();

        public RefereeController(ILogger<RefereeController> logger)
        {
            data.Add("ws_cnclub", Credential.LicenceNumber);
            data.Add("ws_passwd", Credential.Password);
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

        private string CookieReferee()
        {
            using var client = new HttpClient();
            try
            {
                HttpContent formContent = new FormUrlEncodedContent(data);
                var result = client.PostAsync(urlConnexion, formContent);
                string cookie = result.GetAwaiter().GetResult().Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value.First();

                return cookie;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return null;
        }

        [HttpPost("GetCookieReferee")]
        public async Task<string> GetCookieReferee()
        {
            return await GetCookieReferee();
        }

        [HttpPost("GetHtmlReferee")]
        public async Task<string> GetHtmlReferee()
        {
            using var client = new HttpClient();
            try
            {
                var cookie = CookieReferee();
                int position = cookie.IndexOf(";");
                string postParams = cookie.Substring(0, position);
                var result = await client.PostAsync(urlDesignation, new StringContent(postParams));


                var responseString = await result.Content.ReadAsStringAsync();

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(responseString);

                HtmlNode htmlBody = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[2]");
                return htmlBody.InnerHtml;

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return null;
        }
    }
}
