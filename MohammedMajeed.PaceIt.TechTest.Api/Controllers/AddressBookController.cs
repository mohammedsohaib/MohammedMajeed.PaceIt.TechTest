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
    public class AddressBookController : ControllerBase
    {
        private readonly ILogger<AddressBookController> _logger;
        private readonly DataContext _dataContext;

        public AddressBookController(ILogger<AddressBookController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        /// <summary>
        /// Creates a new contact from the posted model.
        /// </summary>
        /// <param name="newContact">The new contact model.</param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(Contact))]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Contact newContact)
        {
            _logger.LogInformation($"{Request.Method} request recieved at {Request.Path}");

            if (newContact == null || !ModelState.IsValid)
            {
                _logger.LogError($"Invalid contact. Please validate contact.");
                return BadRequest(JsonConvert.SerializeObject(ModelState.Values.Select(e => e.Errors).ToList()));
            }

            if (_dataContext.Contacts.Any(c => c.Email.ToLowerInvariant() == newContact.Email.ToLowerInvariant()))
            {
                var error = $"Contact with the email: {newContact.Email} already exists!";
                _logger.LogError(error);
                return BadRequest(error);
            }

            _dataContext.Contacts.Add(
                new Contact
                {
                    FirstName = newContact.FirstName,
                    LastName = newContact.LastName,
                    Phone = newContact.Phone,
                    Email = newContact.Email
                });

            await _dataContext.SaveChangesAsync();

            newContact = _dataContext.Contacts.Single(c => c.Email.ToLowerInvariant() == newContact.Email.ToLowerInvariant());

            return Created("A new contact has been created", newContact);
        }

        /// <summary>
        /// Lists all the contacts.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(List<Contact>))]
        [ProducesResponseType(typeof(List<Contact>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List()
        {
            _logger.LogInformation($"{Request.Method} request recieved at {Request.Path}");
            return Ok(_dataContext.Contacts.ToList());
        }

        /// <summary>
        /// Get a contact by their email.
        /// </summary>
        /// <param name="contactEmail">The contact email.</param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(Contact))]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string contactEmail)
        {
            _logger.LogInformation($"{Request.Method} request recieved at {Request.Path}");

            if (string.IsNullOrWhiteSpace(contactEmail))
            {
                var error = $"Contact email cannot be null or empty";
                _logger.LogError(error);
                return BadRequest(error);
            }

            var contact = _dataContext.Contacts.SingleOrDefault(c => c.Email.ToLowerInvariant() == contactEmail.ToLowerInvariant());
            if (contact == null)
            {
                var error = $"A contact with the email: {contactEmail} cannot be found";
                _logger.LogError(error);
                return NotFound(error);
            }

            return Ok(contact);
        }

        /// <summary>
        /// Updates the contact.
        /// </summary>
        /// <param name="contactEmail">The current contact email.</param>
        /// <param name="updatedContact">The updated contact model.</param>
        /// <returns></returns>
        [HttpPut]
        [Produces("application/json", Type = typeof(Contact))]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(string contactEmail, Contact updatedContact)
        {
            _logger.LogInformation($"{Request.Method} request recieved at {Request.Path}");

            if (string.IsNullOrWhiteSpace(contactEmail))
            {
                var error = $"Contact email cannot be null or empty";
                _logger.LogError(error);
                return BadRequest(error);
            }

            if (updatedContact == null || !ModelState.IsValid)
            {
                _logger.LogError($"Invalid contact. Please validate contact.");
                return BadRequest(JsonConvert.SerializeObject(ModelState.Values.Select(e => e.Errors).ToList()));
            }

            var contact = _dataContext.Contacts.SingleOrDefault(c => c.Email.ToLowerInvariant() == contactEmail.ToLowerInvariant());
            if (contact == null)
            {
                var error = $"Contact with the email: {contactEmail} cannot be found";
                _logger.LogError(error);
                return NotFound(error);
            }

            contact.FirstName = updatedContact.FirstName;
            contact.LastName = updatedContact.LastName;
            contact.Phone = updatedContact.Phone;
            contact.Email = updatedContact.Email;

            await _dataContext.SaveChangesAsync();

            return Ok(contact);
        }

        /// <summary>
        /// Deletes a contact record.
        /// </summary>
        /// <param name="contactEmail">The current contact email.</param>
        /// <returns></returns>
        [HttpDelete]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(string contactEmail)
        {
            _logger.LogInformation($"{Request.Method} request recieved at {Request.Path}");

            if (string.IsNullOrWhiteSpace(contactEmail))
            {
                var error = $"Contact email cannot be null or empty";
                _logger.LogError(error);
                return BadRequest(error);
            }

            var contact = _dataContext.Contacts.SingleOrDefault(c => c.Email.ToLowerInvariant() == contactEmail.ToLowerInvariant());
            if (contact == null)
            {
                var error = $"Contact with the email: {contactEmail} cannot be found";
                _logger.LogError(error);
                return NotFound(error);
            }

            _dataContext.Contacts.Remove(contact);

            await _dataContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
