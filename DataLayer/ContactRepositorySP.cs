using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;

namespace DataLayer
{
    public class ContactRepositorySP : IContactRepository
    {
        private IDbConnection db;
        public ContactRepositorySP(string conString)
        {
            this.db = new SqlConnection(conString);
        }
        public Contact Add(Contact contact)
        {
            throw new NotImplementedException();
        }

        public Contact Find(int id)
        {
           return this.db.Query<Contact>("GetContact", new { Id = id }, commandType: CommandType.StoredProcedure).SingleOrDefault<Contact>();
        }

        public List<Contact> GetAll()
        {
            throw new NotImplementedException();
        }

        public Contact GetFullContact(int id)
        {          

            Contact contact = null;

            using (var multipleResults = this.db.QueryMultiple("GetContact", new { Id = id },commandType:CommandType.StoredProcedure))
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
            throw new NotImplementedException();
        }

        public void Save(Contact contact)
        {
            using (var trScope = new TransactionScope())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", value: contact.Id, dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@FirstName", contact.FirstName);
                parameters.Add("@LastName", contact.LastName);
                parameters.Add("@Company", contact.Company);
                parameters.Add("@Title", contact.Title);
                parameters.Add("@Email", contact.Email);

                this.db.Execute("SaveContact", parameters, commandType: CommandType.StoredProcedure);
                contact.Id = parameters.Get<int>("@Id");

                foreach(var addr in contact.Addresses.Where(a=>!a.IsDeleted))
                {
                    addr.ContactId = contact.Id;
                    var addParams = new DynamicParameters(new
                    {
                        ContactId = addr.ContactId,
                        AddressType=addr.AddressType,
                        StreetAddress = addr.StreetAddress,
                        City = addr.City,
                        StateId=addr.StateId,
                        PostalCode=addr.PostalCode                       
                    });

                    addParams.Add("@Id", addr.Id, DbType.Int32, ParameterDirection.InputOutput);
                    this.db.Execute("SaveAddress", addParams, commandType: CommandType.StoredProcedure);
                    addr.Id = addParams.Get<int>("@Id");
                }

                //foreach(var addr in contact.Addresses.Where(a=>a.IsDeleted))
                //{
                //    this.db.Execute()
                //}

                trScope.Complete();

            }
        }

        public Contact Update(Contact contact)
        {
            throw new NotImplementedException();
        }
    }
}
