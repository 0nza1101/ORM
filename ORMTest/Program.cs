using ORMLib.Constants;
using ORMLib.Generics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;

namespace ORMTest
{
    public class Contacts
    {
        public int contactid { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string email { get; set; }

        public Contacts(int contacId, string name, string address, string email){
            this.contactid = contacId;
            this.name = name;
            this.address = address;
            this.email = email;
        }

        public Contacts()
        {
            this.contactid = 0;
            this.name = "";
            this.address = "";
            this.email = "";
        }
    }

    class Program
    {
        static string team = "Gabrielle, Jordan, Teddy and Can Serkan";
        static void Main(string[] args)
        {
            Console.WriteLine($"Test cases for ORMLib made by { team }");
            PostgreSql();
            MySql();
            MsSql();
        }

        private static void PostgreSql()
        {
            // Creating the database. IF YOU WANT TO TEST IT, DON'T FORGET TO CHANGE THESE PARAMETERS
            DboMapper database = new DboMapper("localhost", "5432", "devdb", "postgres", "root", DatabaseType.PostgreSql);

            //Simple mapping takes only SELECT
            string selectReqWithListFunction = "SELECT * FROM contacts";
            List<Contacts> listMadeByListFunction = database.List<Contacts>(selectReqWithListFunction);
            foreach (var item in listMadeByListFunction)
            {
                Console.WriteLine(item.name, item.address, item.email);
            }
            Console.ReadKey();
            Console.Clear();

            // Calling the Execute Function by giving him a INSERT Statement with parameters
            string insertReq = @"INSERT INTO contacts (name, address, email) VALUES (@name, @address, @email)";
            List<DbParameter> pInsert = new List<DbParameter>();
            pInsert.Add(new NpgsqlParameter("@name", "TEST CASE"));
            pInsert.Add(new NpgsqlParameter("@address", "RUE DU TEST"));
            pInsert.Add(new NpgsqlParameter("@email", "TestEmail@gmail.com"));
            List<Contacts> contactsInsert = database.Execute<Contacts>(insertReq, pInsert);

            // Calling the Execute Function by giving him a SELECT Statement with parameters
            string selectReqWithParameters = @"SELECT * FROM contacts WHERE email = @email";
            List<DbParameter> pSelect = new List<DbParameter>();
            pSelect.Add(new NpgsqlParameter("@email", "TestEmail@gmail.com"));
            List<Contacts> contactsSelectWithParameter = database.Execute<Contacts>(selectReqWithParameters, pSelect);
            foreach (var item in contactsSelectWithParameter)
            {
                Console.WriteLine(item.name, item.address, item.email);
            }
            Console.ReadKey();
            Console.Clear();

            // Calling the Execute Function by giving him a UPDATE Statement with parameters
            string updateReq = @"UPDATE contacts SET name = @name, address = @address, email = @email WHERE email = @oldEmail";
            List<DbParameter> pUpdate = new List<DbParameter>();
            pUpdate.Add(new NpgsqlParameter("@name", "NAME CHANGED FOR TEST"));
            pUpdate.Add(new NpgsqlParameter("@address", "ADDRESS CHANGED FOR TEST"));
            pUpdate.Add(new NpgsqlParameter("@email", "emailChangedForTest@gmail.com"));
            pUpdate.Add(new NpgsqlParameter("@oldEmail", "TestEmail@gmail.com"));
            List<Contacts> contactsUpdate = database.Execute<Contacts>(updateReq, pUpdate);

            // Calling the Execute Function by giving him an SELECT Statement 
            String selectReq = @"SELECT * FROM CONTACTS";
            List<Contacts> contactsSelect = database.Execute<Contacts>(selectReq, pSelect);
            foreach (var item in contactsSelect)
            {
                Console.WriteLine(item.name, item.address, item.email);
            }
            Console.ReadKey();
            Console.Clear();

            // Calling the Execute Function by giving him a DELETE Statement with parameters
            string deleteReq = @"DELETE FROM contacts WHERE address = @address";
            List<DbParameter> pDelete = new List<DbParameter>();
            pDelete.Add(new NpgsqlParameter("@address", "ADDRESS CHANGED FOR TEST"));
            List<Contacts> contactsDelete = database.Execute<Contacts>(deleteReq, pDelete);

            // Calling the Execute Function by giving him a SELECT Statement
            contactsSelect = null;
            contactsSelect = database.Execute<Contacts>(selectReq, pSelect);
            foreach (var item in contactsSelect)
            {
                Console.WriteLine(item.name, item.address, item.email);
            }
            Console.WriteLine("Test is finished, you can click on enter to close this tab.");
            Console.ReadKey();
        }

        private static void MsSql()
        {
            // Creating the database. IF YOU WANT TO TEST IT, DON'T FORGET TO CHANGE THESE PARAMETERS
            DboMapper database = new DboMapper("localhost", "1433", "devdb", "sa", "P@55w0rd", DatabaseType.MSSql);

            //Simple mapping takes only SELECT
            string selectReqWithListFunction = "SELECT * FROM contacts";
            List<Contacts> listMadeByListFunction = database.List<Contacts>(selectReqWithListFunction);
            foreach (var item in listMadeByListFunction)
            {
                Console.WriteLine(item.name, item.address, item.email);
            }
            Console.ReadKey();
            Console.Clear();

            // Calling the Execute Function by giving him a INSERT Statement with parameters
            string insertReq = @"INSERT INTO contacts (name, address, email) VALUES (@name, @address, @email)";
            List<DbParameter> pInsert = new List<DbParameter>();
            pInsert.Add(new SqlParameter("@name", "TEST CASE"));
            pInsert.Add(new SqlParameter("@address", "RUE DU TEST"));
            pInsert.Add(new SqlParameter("@email", "TestEmail@gmail.com"));
            List<Contacts> contactsInsert = database.Execute<Contacts>(insertReq, pInsert);

            // Calling the Execute Function by giving him a SELECT Statement with parameters
            string selectReqWithParameters = @"SELECT * FROM contacts WHERE email = @email";
            List<DbParameter> pSelect = new List<DbParameter>();
            pSelect.Add(new SqlParameter("@email", "TestEmail@gmail.com"));
            List<Contacts> contactsSelectWithParameter = database.Execute<Contacts>(selectReqWithParameters, pSelect);
            foreach (var item in contactsSelectWithParameter)
            {
                Console.WriteLine(item.name, item.address, item.email);
            }
            Console.ReadKey();
            Console.Clear();

            // Calling the Execute Function by giving him a UPDATE Statement with parameters
            string updateReq = @"UPDATE contacts SET name = @name, address = @address, email = @email WHERE email = @oldEmail";
            List<DbParameter> pUpdate = new List<DbParameter>();
            pUpdate.Add(new SqlParameter("@name", "NAME CHANGED FOR TEST"));
            pUpdate.Add(new SqlParameter("@address", "ADDRESS CHANGED FOR TEST"));
            pUpdate.Add(new SqlParameter("@email", "emailChangedForTest@gmail.com"));
            pUpdate.Add(new SqlParameter("@oldEmail", "TestEmail@gmail.com"));
            List<Contacts> contactsUpdate = database.Execute<Contacts>(updateReq, pUpdate);

            // Calling the Execute Function by giving him an SELECT Statement 
            String selectReq = @"SELECT * FROM CONTACTS";
            List<Contacts> contactsSelect = database.Execute<Contacts>(selectReq, pSelect);
            foreach (var item in contactsSelect)
            {
                Console.WriteLine(item.name, item.address, item.email);
            }
            Console.ReadKey();
            Console.Clear();

            // Calling the Execute Function by giving him a DELETE Statement with parameters
            string deleteReq = @"DELETE FROM contacts WHERE address = @address";
            List<DbParameter> pDelete = new List<DbParameter>();
            pDelete.Add(new SqlParameter("@address", "ADDRESS CHANGED FOR TEST"));
            List<Contacts> contactsDelete = database.Execute<Contacts>(deleteReq, pDelete);

            // Calling the Execute Function by giving him a SELECT Statement
            contactsSelect = null;
            contactsSelect = database.Execute<Contacts>(selectReq, pSelect);
            foreach (var item in contactsSelect)
            {
                Console.WriteLine(item.name, item.address, item.email);
            }
            Console.WriteLine("Test is finished, you can click on enter to close this tab.");
            Console.ReadKey();
        }

        private static void MySql()
        {
            // Creating the database. IF YOU WANT TO TEST IT, DON'T FORGET TO CHANGE THESE PARAMETERS
            DboMapper database = new DboMapper("localhost", "", "devdb", "root", "root", DatabaseType.MySql);

            //Simple mapping takes only SELECT
            string selectReqWithListFunction = "SELECT * FROM contacts";
            List<Contacts> listMadeByListFunction = database.List<Contacts>(selectReqWithListFunction);
            foreach (var item in listMadeByListFunction)
            {
                Console.WriteLine(item.name, item.address, item.email);
            }
            Console.ReadKey();
            Console.Clear();

            // Calling the Execute Function by giving him a INSERT Statement with parameters
            string insertReq = @"INSERT INTO contacts (name, address, email) VALUES (@name, @address, @email)";
            List<DbParameter> pInsert = new List<DbParameter>();
            pInsert.Add(new MySqlParameter("@name", "TEST CASE"));
            pInsert.Add(new MySqlParameter("@address", "RUE DU TEST"));
            pInsert.Add(new MySqlParameter("@email", "TestEmail@gmail.com"));
            List<Contacts> contactsInsert = database.Execute<Contacts>(insertReq, pInsert);

            // Calling the Execute Function by giving him a SELECT Statement with parameters
            string selectReqWithParameters = @"SELECT * FROM contacts WHERE email = @email";
            List<DbParameter> pSelect = new List<DbParameter>();
            pSelect.Add(new MySqlParameter("@email", "TestEmail@gmail.com"));
            List<Contacts> contactsSelectWithParameter = database.Execute<Contacts>(selectReqWithParameters, pSelect);
            foreach (var item in contactsSelectWithParameter)
            {
                Console.WriteLine(item.name, item.address, item.email);
            }
            Console.ReadKey();
            Console.Clear();

            // Calling the Execute Function by giving him a UPDATE Statement with parameters
            string updateReq = @"UPDATE contacts SET name = @name, address = @address, email = @email WHERE email = @oldEmail";
            List<DbParameter> pUpdate = new List<DbParameter>();
            pUpdate.Add(new MySqlParameter("@name", "NAME CHANGED FOR TEST"));
            pUpdate.Add(new MySqlParameter("@address", "ADDRESS CHANGED FOR TEST"));
            pUpdate.Add(new MySqlParameter("@email", "emailChangedForTest@gmail.com"));
            pUpdate.Add(new MySqlParameter("@oldEmail", "TestEmail@gmail.com"));
            List<Contacts> contactsUpdate = database.Execute<Contacts>(updateReq, pUpdate);

            // Calling the Execute Function by giving him an SELECT Statement 
            String selectReq = @"SELECT * FROM CONTACTS";
            List<Contacts> contactsSelect = database.Execute<Contacts>(selectReq, pSelect);
            foreach (var item in contactsSelect)
            {
                Console.WriteLine(item.name, item.address, item.email);
            }
            Console.ReadKey();
            Console.Clear();

            // Calling the Execute Function by giving him a DELETE Statement with parameters
            string deleteReq = @"DELETE FROM contacts WHERE address = @address";
            List<DbParameter> pDelete = new List<DbParameter>();
            pDelete.Add(new MySqlParameter("@address", "ADDRESS CHANGED FOR TEST"));
            List<Contacts> contactsDelete = database.Execute<Contacts>(deleteReq, pDelete);

            // Calling the Execute Function by giving him a SELECT Statement
            contactsSelect = null;
            contactsSelect = database.Execute<Contacts>(selectReq, pSelect);
            foreach (var item in contactsSelect)
            {
                Console.WriteLine(item.name, item.address, item.email);
            }
            Console.WriteLine("Test is finished, you can click on enter to close this tab.");
            Console.ReadKey();
        }
    }
}