using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValidateApiController : ControllerBase
    {
        private readonly ILogger<ValidateApiController> _logger;

        public ValidateApiController(ILogger<ValidateApiController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("UsingSamlToken")]
        public ActionResult<string> Get()
        {
            return Ok("Awesome! API has authenticated the request from ASPNetCore Web app.");
        }
    }
}
