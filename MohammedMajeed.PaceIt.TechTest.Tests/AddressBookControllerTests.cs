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
                first_name = "Unit",
                last_name = "Tester",
                phone = "00000000000",
                email = "unit@tester"
            };

            var response = await addressBookController.Create(newContact);
            var objectResult = response as CreatedResult;
            var result = objectResult.Value;

            Assert.IsInstanceOf<CreatedResult>(objectResult);
            Assert.AreEqual((int)HttpStatusCode.Created, objectResult.StatusCode);
            Assert.AreEqual(typeof(Contact), result.GetType());
            Assert.AreEqual(newContact.first_name, ((Contact)result).first_name);
            Assert.AreEqual(newContact.last_name, ((Contact)result).last_name);
            Assert.AreEqual(newContact.phone, ((Contact)result).phone);
            Assert.AreEqual(newContact.email, ((Contact)result).email);
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
            var contact = TestCustomerData().Single(c => c.email == "david.platt@corrie.co.uk");

            var response = await addressBookController.Get(contact.email);
            var objectResult = response as OkObjectResult;
            var result = objectResult.Value;

            Assert.IsInstanceOf<OkObjectResult>(objectResult);
            Assert.AreEqual((int)HttpStatusCode.OK, objectResult.StatusCode);
            Assert.AreEqual(typeof(Contact), result.GetType());
            Assert.AreEqual(contact.first_name, ((Contact)result).first_name);
            Assert.AreEqual(contact.last_name, ((Contact)result).last_name);
            Assert.AreEqual(contact.phone, ((Contact)result).phone);
            Assert.AreEqual(contact.email, ((Contact)result).email);
        }

        [Test]
        public async Task Update()
        {
            var contact = TestCustomerData().Single(c => c.email == "rita.sullivan@corrie.co.uk");

            var updatedContact = new Contact
            {
                first_name = "Unit",
                last_name = "Tester",
                phone = "00000000000",
                email = "unit@tester"
            };

            var response = await addressBookController.Update(contact.email, updatedContact);
            var objectResult = response as OkObjectResult;
            var result = objectResult.Value;

            Assert.IsInstanceOf<OkObjectResult>(objectResult);
            Assert.AreEqual((int)HttpStatusCode.OK, objectResult.StatusCode);
            Assert.AreEqual(typeof(Contact), result.GetType());
            Assert.AreNotEqual(contact.first_name, ((Contact)result).first_name);
            Assert.AreEqual(updatedContact.first_name, ((Contact)result).first_name);
            Assert.AreNotEqual(contact.last_name, ((Contact)result).last_name);
            Assert.AreEqual(updatedContact.last_name, ((Contact)result).last_name);
            Assert.AreNotEqual(contact.phone, ((Contact)result).phone);
            Assert.AreEqual(updatedContact.phone, ((Contact)result).phone);
            Assert.AreNotEqual(contact.email, ((Contact)result).email);
            Assert.AreEqual(updatedContact.email, ((Contact)result).email);
        }

        [Test]
        public async Task Delete()
        {
            var contact = TestCustomerData().Single(c => c.email == "steve.mcdonald@corrie.co.uk");

            var response = await addressBookController.Delete(contact.email);
            var noContentResult = response as NoContentResult;

            Assert.IsInstanceOf<NoContentResult>(noContentResult);
            Assert.AreEqual((int)HttpStatusCode.NoContent, noContentResult.StatusCode);

            response = await addressBookController.Get(contact.email);
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
                    first_name = "David",
                    last_name = "Platt",
                    phone = "01913478234",
                    email = "david.platt@corrie.co.uk"
                },
                new Contact
                {
                    first_name = "Jason",
                    last_name = "Grimshaw",
                    phone = "01913478123",
                    email = "jason.grimshaw@corrie.co.uk"
                },
                new Contact
                {
                    first_name = "Ken",
                    last_name = "Barlow",
                    phone = "019134784929",
                    email = "ken.barlow@corrie.co.uk"
                },
                new Contact
                {
                    first_name = "Rita", 
                    last_name = "Sullivan", 
                    phone = "01913478555", 
                    email = "rita.sullivan@corrie.co.uk"},
                new Contact
                {
                    first_name = "Steve", 
                    last_name = "McDonald", 
                    phone = "01913478555", 
                    email = "steve.mcdonald@corrie.co.uk"}
            };
        }

        #endregion Test Data
    }
}