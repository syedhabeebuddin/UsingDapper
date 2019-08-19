using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Transactions;

namespace DataLayer
{
    public class ContactRepository : IContactRepository
    {
        private IDbConnection db;
        public ContactRepository(string connectionString)
        {
            this.db = new SqlConnection(connectionString);

        }

        public Contact Add(Contact contact)
        {
            var sql =
                "Insert into Contacts (FirstName,LastName,Email,Company,Title) values(@FirstName,@LastName,@Email,@Company,@Title); " +
                "Select Cast(SCOPE_IDENTITY() as int)";

            var id = this.db.Query<int>(sql, contact).Single();
            contact.Id = id;
            return contact;
        }

        public Address Add(Address address)
        {
            var sql =
                "Insert into Addresses (ContactId,AddressType,StreetAddress, City, StateId,PostalCode) values(@ContactId,@AddressType,@StreetAddress,@City,@StateId,@PostalCode); " +
                "Select Cast(SCOPE_IDENTITY() as int)";

            var id = this.db.Query<int>(sql, address).Single();
            address.Id = id;
            return address;
        }

        public Contact Find(int id)
        {
            return this.db.Query<Contact>("Select * from Contacts where Id=@Id", new { id }).SingleOrDefault();
        }

        public List<Contact> GetAll()
        {
            return this.db.Query<Contact>("Select * From Contacts").ToList();
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
                if (contact != null && addresses != null)
                {
                    contact.Addresses.AddRange(addresses);
                }
            }
            return contact;

        }

        public int Remove(int id)
        {
            return this.db.Execute("Delete from Contacts where Id=@Id", new { id });
        }

        public void Save(Contact contact)
        {
            this.db.Open();
            using (var trScope = new TransactionScope())
            {
                
                if (contact.IsNew)
                {
                    this.Add(contact);
                }
                else
                {
                    this.Update(contact);
                }

                foreach (var addr in contact.Addresses.Where(a => !a.IsDeleted))
                {
                    addr.ContactId = contact.Id;
                    if (addr.IsNew)
                    {
                        this.Add(addr);
                    }
                    else
                    {
                        this.Update(addr);
                    }
                }

                foreach (var addr in contact.Addresses.Where(a => a.IsDeleted))
                {
                    this.db.Execute("Delete from Addresses where Id=@Id", new { Id = addr.Id });
                }
                trScope.Complete();
            }
        }

        public Contact Update(Contact contact)
        {
            var sql = "Update Contacts Set FirstName = @FirstName,LastName=@LastName,Email=@Email,Company=@Company,Title=@Title where Id=@Id";
            this.db.Execute(sql, contact);

            return contact;
        }

        public Address Update(Address address)
        {
            var sql = "Update Addresses Set AddressType = @AddressType," +
                "StreetAddress=@StreetAddress," +
                "City=@City,StateId=@StateId," +
                "PostalCode=@PostalCode where Id=@Id";
            this.db.Execute(sql, address);

            return address;
        }
    }
}
