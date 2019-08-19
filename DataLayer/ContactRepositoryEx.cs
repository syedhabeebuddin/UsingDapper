using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataLayer
{
    public class ContactRepositoryEx
    {
        private IDbConnection db;
        public ContactRepositoryEx(string connectionString)
        {
            this.db = new SqlConnection(connectionString);

        }

        public List<Contact> GetContactsByIds(params int[] ids)
        {
            return this.db.Query<Contact>("Select * from Contacts where Id in @Ids",new { Ids=ids}).ToList();
        }
    }
}
