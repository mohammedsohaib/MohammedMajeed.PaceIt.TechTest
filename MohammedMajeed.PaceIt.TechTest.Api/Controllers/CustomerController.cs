using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MohammedMajeed.PaceIt.TechTest.Data;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MohammedMajeed.PaceIt.TechTest.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly DataContext _dataContext;

        public CustomerController(ILogger<CustomerController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        /// <summary>
        /// Creates a customer record from the new customer model.
        /// </summary>
        /// <param name="newCustomerModel">The new customer model.</param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(Customer))]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(Customer newCustomerModel)
        {
            _logger.LogInformation($"{Request.Method} request recieved at {Request.Path}");

            if (newCustomerModel == null || !ModelState.IsValid)
            {
                _logger.LogError($"Invalid model. Please validate model.");
                return BadRequest(JsonConvert.SerializeObject(ModelState.Values.Select(e => e.Errors).ToList()));
            }

            if (_dataContext.Customers.Any(c => c.Email.ToLowerInvariant() == newCustomerModel.Email.ToLowerInvariant()))
            {
                var error = $"Customer with the email: {newCustomerModel.Email} already exists!";
                _logger.LogError(error);
                return BadRequest(error);
            }

            await _dataContext.Customers.AddAsync(
                new Customer
                {
                    FirstName = newCustomerModel.FirstName,
                    LastName = newCustomerModel.LastName,
                    Phone = newCustomerModel.Phone,
                    Email = newCustomerModel.Email
                });

            await _dataContext.SaveChangesAsync();

            var newCustomer = _dataContext.Customers.Single(c => c.Email.ToLowerInvariant() == newCustomerModel.Email.ToLowerInvariant());

            return Ok(newCustomer);
        }

        /// <summary>
        /// Lists all the customers records in the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(List<Customer>))]
        [ProducesResponseType(typeof(List<Customer>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List()
        {
            _logger.LogInformation($"{Request.Method} request recieved at {Request.Path}");
            return Ok(_dataContext.Customers);
        }

        /// <summary>
        /// Get a customer by their email address.
        /// </summary>
        /// <param name="email">The customer email email address.</param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(Customer))]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string email)
        {
            _logger.LogInformation($"{Request.Method} request recieved at {Request.Path}");

            if (string.IsNullOrWhiteSpace(email))
            {
                var error = $"Customer email cannot be null or empty";
                _logger.LogError(error);
                return BadRequest(error);
            }

            var customer = _dataContext.Customers.SingleOrDefault(c => c.Email.ToLowerInvariant() == email.ToLowerInvariant());
            if (customer == null)
            {
                var error = $"Customer with the email: {email} cannot be found";
                _logger.LogError(error);
                return NotFound(error);
            }

            return Ok(customer);
        }

        /// <summary>
        /// Updates the customer record.
        /// </summary>
        /// <param name="email">The current customer email.</param>
        /// <param name="updateModel">The updated customer model.</param>
        /// <returns></returns>
        [HttpPut]
        [Produces("application/json", Type = typeof(Customer))]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(string email, Customer updateModel)
        {
            _logger.LogInformation($"{Request.Method} request recieved at {Request.Path}");

            if (string.IsNullOrWhiteSpace(email))
            {
                var error = $"Customer email cannot be null or empty";
                _logger.LogError(error);
                return BadRequest(error);
            }

            if (updateModel == null || !ModelState.IsValid)
            {
                _logger.LogError($"Invalid model. Please validate model.");
                return BadRequest(JsonConvert.SerializeObject(ModelState.Values.Select(e => e.Errors).ToList()));
            }

            var customer = _dataContext.Customers.SingleOrDefault(c => c.Email.ToLowerInvariant() == email.ToLowerInvariant());
            if (customer == null)
            {
                var error = $"Customer with the email: {email} cannot be found";
                _logger.LogError(error);
                return NotFound(error);
            }

            customer.FirstName = updateModel.FirstName;
            customer.LastName = updateModel.LastName;
            customer.Phone = updateModel.Phone;
            customer.Email = updateModel.Email;

            await _dataContext.SaveChangesAsync();

            return Ok(customer);
        }

        /// <summary>
        /// Deletes a customer record by their email address.
        /// </summary>
        /// <param name="email">The customer email.</param>
        /// <returns></returns>
        [HttpDelete]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(string email)
        {
            _logger.LogInformation($"{Request.Method} request recieved at {Request.Path}");

            if (string.IsNullOrWhiteSpace(email))
            {
                var error = $"Customer email cannot be null or empty";
                _logger.LogError(error);
                return BadRequest(error);
            }

            var customer = _dataContext.Customers.SingleOrDefault(c => c.Email.ToLowerInvariant() == email.ToLowerInvariant());
            if (customer == null)
            {
                var error = $"Customer with the email: {email} cannot be found";
                _logger.LogError(error);
                return NotFound(error);
            }

            _dataContext.Customers.Remove(customer);

            await _dataContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
