using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        [HttpGet("GetHtmlReferee")]
        public async Task<string> GetHtmlReferee()
        {
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = new Uri(urlDesignation) })
                try
                {

                    var cookie = CookieReferee();
                    int endPosition = cookie.IndexOf(";");
                    int startPosition = cookie.IndexOf("=");
                    var PHPSESSID = cookie.Substring(0, endPosition);
                    PHPSESSID = PHPSESSID.Substring(startPosition + 1);
                    cookieContainer.Add(new Uri(urlDesignation), new Cookie("PHPSESSID", PHPSESSID));

                    var result = await client.GetAsync(urlDesignation);


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
