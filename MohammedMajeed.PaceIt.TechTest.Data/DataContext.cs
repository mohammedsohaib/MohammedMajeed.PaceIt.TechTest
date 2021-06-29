
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MohammedMajeed.PaceIt.TechTest.Data
{
    public class DataContext
    {
        private string jsonFilePath { get; set; }

        public List<Contact> Contacts { get; set; }

        public DataContext()
        {
            Contacts = new List<Contact>();
        }

        public static async Task Initialise(IServiceProvider serviceProvider, string jsonFilePath)
        {
            var dataContext = (DataContext)serviceProvider.GetService(typeof(DataContext));

            if (File.Exists(jsonFilePath))
            {
                dataContext.jsonFilePath = jsonFilePath;

                if (dataContext.Contacts == null || !dataContext.Contacts.Any())
                {
                    using (StreamReader file = File.OpenText(jsonFilePath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        dataContext.Contacts = (List<Contact>)serializer.Deserialize(file, dataContext.Contacts.GetType());
                    }
                }
            }
        }

        public async Task SaveChangesAsync()
        {
            if (File.Exists(jsonFilePath))
            {
                using (TextWriter file = File.CreateText(jsonFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, Contacts);
                }
            }
        }
    }
}