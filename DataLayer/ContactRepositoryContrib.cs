using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataLayer
{
    public class ContactRepositoryContrib : IContactRepository
    {
        private IDbConnection db;
        public ContactRepositoryContrib(string conString)
        {
            db = new SqlConnection(conString);
        }


        public Contact Add(Contact contact)
        {
            var id = this.db.Insert(contact);
            contact.Id = (int) id;
            return contact;
        }

        public Contact Find(int id)
        {
            return this.db.Get<Contact>(id);
        }

        public List<Contact> GetAll()
        {
            return this.db.GetAll<Contact>().ToList();
        }

        public Contact GetFullContact(int id)
        {
            var sql = "Select * from Contacts where Id=@Id; " +
                    "Select * from Addresses where ContactId=@Id";

            Contact contact = null;

            using (var multipleResults = this.db.QueryMultiple(sql, new { Id = id }))
            {
                contact = multipleResults.Read<Contact>().SingleOrDefault<Contact>();
                var addresses = multipleResults.Read<Address>().ToList();
                if(contact != null && addresses != null)
                {
                    contact.Addresses.AddRange(addresses);
                }
            }
            return contact;

        }

        public int Remove(int id)
        {
            bool num= this.db.Delete<Contact>(new Contact { Id = id });
            return 1;
        }

        public void Save(Contact contact)
        {
            throw new NotImplementedException();
        }

        public Contact Update(Contact contact)
        {
            this.db.Update<Contact>(contact);
            return contact;
        }
    }
}
