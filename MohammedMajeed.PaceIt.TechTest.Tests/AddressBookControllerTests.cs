using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using MohammedMajeed.PaceIt.TechTest.Api.Controllers;
using MohammedMajeed.PaceIt.TechTest.Data;

using Moq;

using NUnit.Framework;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MohammedMajeed.PaceIt.TechTest.Tests
{
    public class AddressBookControllerTests
    {
        private AddressBookController addressBookController;
        private DataContext dataContext;

        [SetUp]
        public void Setup()
        {
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Path = "/";
            mockHttpContext.Request.Method = "TEST";

            var mockLogger = new Mock<ILogger<AddressBookController>>();
            dataContext = new DataContext { Contacts = TestCustomerData() };

            addressBookController = new AddressBookController(mockLogger.Object, dataContext)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = mockHttpContext
                }
            };
        }

        [TearDown]
        public void Cleanup()
        {
            dataContext.Contacts = null;
            addressBookController = null;
        }

        [Test]
        public async Task Create()
        {
            var newContact = new Contact
            {
                FirstName = "Unit",
                LastName = "Tester",
                Phone = "00000000000",
                Email = "unit@tester"
            };

            var response = await addressBookController.Create(newContact);
            var objectResult = response as CreatedResult;
            var result = objectResult.Value;

            Assert.IsInstanceOf<CreatedResult>(objectResult);
            Assert.AreEqual((int)HttpStatusCode.Created, objectResult.StatusCode);
            Assert.AreEqual(typeof(Contact), result.GetType());
            Assert.AreEqual(newContact.FirstName, ((Contact)result).FirstName);
            Assert.AreEqual(newContact.LastName, ((Contact)result).LastName);
            Assert.AreEqual(newContact.Phone, ((Contact)result).Phone);
            Assert.AreEqual(newContact.Email, ((Contact)result).Email);
        }

        [Test]
        public async Task List()
        {
            var contactsCount = TestCustomerData().Count();

            var response = await addressBookController.List();
            var objectResult = response as OkObjectResult;
            var result = objectResult.Value;

            Assert.IsInstanceOf<OkObjectResult>(objectResult);
            Assert.AreEqual((int)HttpStatusCode.OK, objectResult.StatusCode);
            Assert.AreEqual(typeof(List<Contact>), result.GetType());
            Assert.AreEqual(contactsCount, ((List<Contact>)result).Count);
        }

        [Test]
        public async Task Get()
        {
            var contact = TestCustomerData().Single(c => c.Email == "david.platt@corrie.co.uk");

            var response = await addressBookController.Get(contact.Email);
            var objectResult = response as OkObjectResult;
            var result = objectResult.Value;

            Assert.IsInstanceOf<OkObjectResult>(objectResult);
            Assert.AreEqual((int)HttpStatusCode.OK, objectResult.StatusCode);
            Assert.AreEqual(typeof(Contact), result.GetType());
            Assert.AreEqual(contact.FirstName, ((Contact)result).FirstName);
            Assert.AreEqual(contact.LastName, ((Contact)result).LastName);
            Assert.AreEqual(contact.Phone, ((Contact)result).Phone);
            Assert.AreEqual(contact.Email, ((Contact)result).Email);
        }

        [Test]
        public async Task Update()
        {
            var contact = TestCustomerData().Single(c => c.Email == "rita.sullivan@corrie.co.uk");

            var updatedContact = new Contact
            {
                FirstName = "Unit",
                LastName = "Tester",
                Phone = "00000000000",
                Email = "unit@tester"
            };

            var response = await addressBookController.Update(contact.Email, updatedContact);
            var objectResult = response as OkObjectResult;
            var result = objectResult.Value;

            Assert.IsInstanceOf<OkObjectResult>(objectResult);
            Assert.AreEqual((int)HttpStatusCode.OK, objectResult.StatusCode);
            Assert.AreEqual(typeof(Contact), result.GetType());
            Assert.AreNotEqual(contact.FirstName, ((Contact)result).FirstName);
            Assert.AreEqual(updatedContact.FirstName, ((Contact)result).FirstName);
            Assert.AreNotEqual(contact.LastName, ((Contact)result).LastName);
            Assert.AreEqual(updatedContact.LastName, ((Contact)result).LastName);
            Assert.AreNotEqual(contact.Phone, ((Contact)result).Phone);
            Assert.AreEqual(updatedContact.Phone, ((Contact)result).Phone);
            Assert.AreNotEqual(contact.Email, ((Contact)result).Email);
            Assert.AreEqual(updatedContact.Email, ((Contact)result).Email);
        }

        [Test]
        public async Task Delete()
        {
            var contact = TestCustomerData().Single(c => c.Email == "steve.mcdonald@corrie.co.uk");

            var response = await addressBookController.Delete(contact.Email);
            var noContentResult = response as NoContentResult;

            Assert.IsInstanceOf<NoContentResult>(noContentResult);
            Assert.AreEqual((int)HttpStatusCode.NoContent, noContentResult.StatusCode);

            response = await addressBookController.Get(contact.Email);
            var notFoundObjectResult = response as NotFoundObjectResult;

            Assert.IsInstanceOf<NotFoundObjectResult>(notFoundObjectResult);
            Assert.AreEqual((int)HttpStatusCode.NotFound, notFoundObjectResult.StatusCode);
        }

        #region Test Data

        private List<Contact> TestCustomerData()
        {
            return new List<Contact>()
            {
                new Contact
                {
                    FirstName = "David",
                    LastName = "Platt",
                    Phone = "01913478234",
                    Email = "david.platt@corrie.co.uk"
                },
                new Contact
                {
                    FirstName = "Jason",
                    LastName = "Grimshaw",
                    Phone = "01913478123",
                    Email = "jason.grimshaw@corrie.co.uk"
                },
                new Contact
                {
                    FirstName = "Ken",
                    LastName = "Barlow",
                    Phone = "019134784929",
                    Email = "ken.barlow@corrie.co.uk"
                },
                new Contact
                {
                    FirstName = "Rita",
                    LastName = "Sullivan",
                    Phone = "01913478555",
                    Email = "rita.sullivan@corrie.co.uk"
                },
                new Contact
                {
                    FirstName = "Steve",
                    LastName = "McDonald",
                    Phone = "01913478555",
                    Email = "steve.mcdonald@corrie.co.uk"
                }
            };
        }

        #endregion Test Data
    }
}