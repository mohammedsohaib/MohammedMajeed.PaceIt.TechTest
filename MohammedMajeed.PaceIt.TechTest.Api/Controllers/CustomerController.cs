using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MohammedMajeed.PaceIt.TechTest.Data;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MohammedMajeed.PaceIt.TechTest.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// This endpoint lists all the customers available in the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(List<Customer>))]
        [ProducesResponseType(typeof(List<Customer>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List()
        {
            _logger.LogInformation($"{Request.Method} request recieved at {Request.Path}");

            return Ok();
        }
    }
}
