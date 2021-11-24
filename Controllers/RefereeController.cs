using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VolleyApi.Model;

namespace VolleyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RefereeController : ControllerBase
    {
        private readonly ILogger<RefereeController> _logger;

        public RefereeController(ILogger<RefereeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Referee Get()
        {
            return new Referee
            {
                UserId = "2*****6",
                Password = "******"
            };
        }
    }
}
