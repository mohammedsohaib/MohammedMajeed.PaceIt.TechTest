using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;

namespace MohammedMajeed.PaceIt.TechTest.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }

        public static void Initialise(IServiceProvider serviceProvider)
        {
            using (var context = new DataContext(serviceProvider.GetRequiredService<DbContextOptions<DataContext>>()))
            {
                if (context.Customers.Any())
                {
                    return;
                }

                context.Customers.AddRange(
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
                    });
            };
        }
    }
}
