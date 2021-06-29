using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class CustomerControllerTests
    {
        private CustomerController customerController;
        private DataContext dataContext;

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "UnitTestDatabase")
            .Options;

            using (var context = new DataContext(dbContextOptions))
            {
                foreach (var customer in TestCustomerData())
                {
                    context.Customers.Add(customer);
                }

                context.SaveChanges();
            }

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Path = "/";
            mockHttpContext.Request.Method = "TEST";

            var mockLogger = new Mock<ILogger<CustomerController>>();
            dataContext = new DataContext(dbContextOptions);

            customerController = new CustomerController(mockLogger.Object, dataContext)
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
            dataContext.Database.EnsureDeleted();
            customerController = null;
        }

        [Test]
        public async Task Create()
        {
            var newModel = new Customer
            {
                FirstName = "Unit",
                LastName = "Tester",
                Phone = "00000000000",
                Email = "unit@tester"
            };

            var response = await customerController.Create(newModel);
            var objectResult = response as CreatedResult;
            var result = objectResult.Value;

            Assert.IsInstanceOf<CreatedResult>(objectResult);
            Assert.AreEqual((int)HttpStatusCode.Created, objectResult.StatusCode);
            Assert.AreEqual(typeof(Customer), result.GetType());
            Assert.AreEqual(newModel.FirstName, ((Customer)result).FirstName);
            Assert.AreEqual(newModel.LastName, ((Customer)result).LastName);
            Assert.AreEqual(newModel.Phone, ((Customer)result).Phone);
            Assert.AreEqual(newModel.Email, ((Customer)result).Email);
        }

        [Test]
        public async Task List()
        {
            var testCustomersCount = TestCustomerData().Count();

            var response = await customerController.List();
            var objectResult = response as OkObjectResult;
            var result = objectResult.Value;

            Assert.IsInstanceOf<OkObjectResult>(objectResult);
            Assert.AreEqual((int)HttpStatusCode.OK, objectResult.StatusCode);
            Assert.AreEqual(typeof(List<Customer>), result.GetType());
            Assert.AreEqual(testCustomersCount, ((List<Customer>)result).Count);
        }

        [Test]
        public async Task Get()
        {
            var testCustomer = TestCustomerData().Single(c => c.Email == "david.platt@corrie.co.uk");

            var response = await customerController.Get(testCustomer.Email);
            var objectResult = response as OkObjectResult;
            var result = objectResult.Value;

            Assert.IsInstanceOf<OkObjectResult>(objectResult);
            Assert.AreEqual((int)HttpStatusCode.OK, objectResult.StatusCode);
            Assert.AreEqual(typeof(Customer), result.GetType());
            Assert.AreEqual(testCustomer.FirstName, ((Customer)result).FirstName);
            Assert.AreEqual(testCustomer.LastName, ((Customer)result).LastName);
            Assert.AreEqual(testCustomer.Phone, ((Customer)result).Phone);
            Assert.AreEqual(testCustomer.Email, ((Customer)result).Email);
        }

        [Test]
        public async Task Update()
        {
            var testCustomer = TestCustomerData().Single(c => c.Email == "rita.sullivan@corrie.co.uk");

            var updateModel = new Customer
            {
                FirstName = "Unit",
                LastName = "Tester",
                Phone = "00000000000",
                Email = "unit@tester"
            };

            var response = await customerController.Update(testCustomer.Email, updateModel);
            var objectResult = response as OkObjectResult;
            var result = objectResult.Value;

            Assert.IsInstanceOf<OkObjectResult>(objectResult);
            Assert.AreEqual((int)HttpStatusCode.OK, objectResult.StatusCode);
            Assert.AreEqual(typeof(Customer), result.GetType());
            Assert.AreNotEqual(testCustomer.FirstName, ((Customer)result).FirstName);
            Assert.AreEqual(updateModel.FirstName, ((Customer)result).FirstName);
            Assert.AreNotEqual(testCustomer.LastName, ((Customer)result).LastName);
            Assert.AreEqual(updateModel.LastName, ((Customer)result).LastName);
            Assert.AreNotEqual(testCustomer.Phone, ((Customer)result).Phone);
            Assert.AreEqual(updateModel.Phone, ((Customer)result).Phone);
            Assert.AreNotEqual(testCustomer.Email, ((Customer)result).Email);
            Assert.AreEqual(updateModel.Email, ((Customer)result).Email);
        }

        [Test]
        public async Task Delete()
        {
            var testCustomer = TestCustomerData().Single(c => c.Email == "steve.mcdonald@corrie.co.uk");

            var response = await customerController.Delete(testCustomer.Email);
            var noContentResult = response as NoContentResult;

            Assert.IsInstanceOf<NoContentResult>(noContentResult);
            Assert.AreEqual((int)HttpStatusCode.NoContent, noContentResult.StatusCode);

            response = await customerController.Get(testCustomer.Email);
            var notFoundObjectResult = response as NotFoundObjectResult;

            Assert.IsInstanceOf<NotFoundObjectResult>(notFoundObjectResult);
            Assert.AreEqual((int)HttpStatusCode.NotFound, notFoundObjectResult.StatusCode);
        }

        #region Test Data

        private List<Customer> TestCustomerData()
        {
            return new List<Customer>()
            {
                new Customer
                {
                    FirstName = "David",
                    LastName = "Platt",
                    Phone = "01913478234",
                    Email = "david.platt@corrie.co.uk"
                },
                new Customer
                {
                    FirstName = "Jason",
                    LastName = "Grimshaw",
                    Phone = "01913478123",
                    Email = "jason.grimshaw@corrie.co.uk"
                },
                new Customer
                {
                    FirstName = "Ken",
                    LastName = "Barlow",
                    Phone = "019134784929",
                    Email = "ken.barlow@corrie.co.uk"
                },
                new Customer
                {
                    FirstName = "Rita",
                    LastName = "Sullivan",
                    Phone = "01913478555",
                    Email = "rita.sullivan@corrie.co.uk"
                },
                new Customer
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