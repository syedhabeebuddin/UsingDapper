using DataLayer;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Runner
{
    class Program
    {
        private static IConfigurationRoot config;
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            Initialize();

            GetList();

            //GetAll();
            //Console.ReadLine();
            //int id = Insert();
            //Console.ReadLine();
            //Find(id);
            //Console.ReadLine();
            //Update();
            //Console.ReadLine();
            //Delete();

            var repo = CreateRepository();
            var contacts= repo.GetFullContact(1);
            contacts.Output();
            Console.ReadLine();
        }

        static void GetList()
        {
            var repository = CreateRepositoryEx();
            var contacts = repository.GetContactsByIds(1,2,3,4);

            Console.WriteLine($"count : {contacts.Count}");
            contacts.Output();

        }

        static void GetAll()
        {
            var repository = CreateRepository();
            var contacts = repository.GetAll();

            Console.WriteLine($"count : {contacts.Count}");
            contacts.Output();
        }

        static int Insert()
        {
            IContactRepository repo = CreateRepository();
            var contact = new Contact
            {
                FirstName = "aaa",
                LastName = "bb",
                Email = "aa@jj.com",
                Company = "ccc",
                Title = "tt"
            };

            var address = new Address
            {
                AddressType = "Home",
                City = "Hyderabad",
                PostalCode = "1122",
                StateId = 1,
                StreetAddress="wage street"
            };

            contact.Addresses.Add(address);

            //repo.Add(contact);
            repo.Save(contact);

            Console.WriteLine("Contact Inserted");
            Console.WriteLine($"New Contact Id : {contact.Id}");

            return contact.Id;
        }

        static void Find(int id)
        {
            var repository = CreateRepository();
            var contacts = repository.Find(id);

            
            contacts.Output();
        }

        static void Update()
        {
            var repository = CreateRepository();
            var contacts = repository.Find(8);

            contacts.LastName = "mmmmm";
            repository.Update(contacts);

            contacts.Output();
        }

        static void Delete()
        {
            var repository = CreateRepository();          

            
            repository.Remove(7);

            
        }

        private static void Initialize()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            config = builder.Build();
        }

        private static IContactRepository CreateRepository()
        {
            //return new ContactRepository(config.GetConnectionString("DefaultConnection"));
            //return new ContactRepositoryContrib(config.GetConnectionString("DefaultConnection"));
            return new ContactRepositorySP(config.GetConnectionString("DefaultConnection"));
        }

        private static ContactRepositoryEx CreateRepositoryEx()
        {
            //return new ContactRepository(config.GetConnectionString("DefaultConnection"));
            //return new ContactRepositoryContrib(config.GetConnectionString("DefaultConnection"));
            return new ContactRepositoryEx(config.GetConnectionString("DefaultConnection"));
        }
    }
}
