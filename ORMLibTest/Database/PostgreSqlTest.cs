using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using ORMLibTest.Modele;
using ORMLib.exception;
using ORMLib.Database;
using Npgsql;



namespace ORMLibTest
{
    [TestClass]
    public class PostgreSqlTest
    {
        private PostgreSql database = new PostgreSql("localhost","devdb","postgres"," ");

        [TestMethod]
        public void TestListWithSelectStatement()
        {
            //GIVEN
            string selectReqWithListFunction = "SELECT * FROM Contacts";

            //WHEN
            List<Contacts> listMadeByListFunction = database.Select<Contacts>(selectReqWithListFunction);
            //THEN

            Assert.IsNotNull(listMadeByListFunction);
            Assert.IsTrue(listMadeByListFunction.Count > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(DatabaseException), "Problème lors de l'exécution de la requête PostgreSql :")]
        public void TestListWithWrongSelectStatement()
        {
            //GIVEN
            string selectReqWithListFunction = "SELECTZZZZ * FROM Contacts";

            //WHEN
            List<Contacts> listMadeByListFunction = database.Select<Contacts>(selectReqWithListFunction);

            //THEN
        }

        [TestMethod]
        public void TestListWithSelectStatementThatReturnsNothing()
        {
            //GIVEN
            string selectReq = "SELECT * FROM contacts WHERE NAME = 'THIS REQUEST SHALL NOT PASS'";

            //WHEN
            List<Contacts> listMadeByListFunction = database.Select<Contacts>(selectReq);

            //THEN
            Assert.IsTrue(listMadeByListFunction.Count == 0);

        }

        [TestMethod]
        public void TestExecuteWithSelectStatementAndWithParameters()
        {
            //GIVEN
            string selectReqWithParameters = @"SELECT * FROM contacts WHERE email = @email";
            List<DbParameter> selectParameters = new List<DbParameter>();
            selectParameters.Add(new NpgsqlParameter("@email", "canserkan.uren@gmail.com"));

            //WHEN
            List<Contacts> contactsSelectWithParameter = database.Execute<Contacts>(selectReqWithParameters, selectParameters);

            //THEN
            Contacts contact = contactsSelectWithParameter.First();

            Assert.AreEqual("canserkan.uren@gmail.com", contact.email);
            Assert.IsNotNull(contactsSelectWithParameter);
            Assert.IsNotNull(contact);
        }

        [TestMethod]
        public void TestExecuteWithInsertStatementAndWithParameters()
        {
            //GIVEN
            string insertReq = @"INSERT INTO contacts (name, address, email) VALUES (@name, @address, @email)";
            List<DbParameter> insertParameters = new List<DbParameter>();
            insertParameters.Add(new NpgsqlParameter("@name", "TEST CASE"));
            insertParameters.Add(new NpgsqlParameter("@address", "RUE DU TEST"));
            insertParameters.Add(new NpgsqlParameter("@email", "TestEmail@gmail.com"));

            //WHEN
            List<Contacts> contactsInsertWithParameter = database.Execute<Contacts>(insertReq, insertParameters);

            //THEN            
            Assert.IsTrue((contactsInsertWithParameter.Count == 0));
        }

        [TestMethod]
        public void TestExecuteWithUpdateStatementAndWithParameters()
        {
            //GIVEN
            string updateReq = @"UPDATE contacts SET name = @name, address = @address, @email = email WHERE email = @oldEmail";
            List<DbParameter> updateParameters = new List<DbParameter>();
            updateParameters.Add(new NpgsqlParameter("@name", "NAME CHANGED FOR TEST"));
            updateParameters.Add(new NpgsqlParameter("@address", "ADDRESS CHANGED FOR TEST"));
            updateParameters.Add(new NpgsqlParameter("@email", "emailChangedForTest@gmail.com"));
            updateParameters.Add(new NpgsqlParameter("@oldEmail", "TestEmail@gmail.com"));

            //WHEN
            List<Contacts> contactsUpdate = database.Execute<Contacts>(updateReq, updateParameters);

            //THEN
            Assert.IsTrue((contactsUpdate.Count == 0));
        }

        [TestMethod]
        public void TestExecuteWithDeleteStatementAndWithParameters()
        {
            //GIVEN
            string deleteReq = @"DELETE FROM contacts WHERE address = @address";
            List<DbParameter> deleteParameters = new List<DbParameter>();
            deleteParameters.Add(new NpgsqlParameter("@address", "ADDRESS CHANGED FOR TEST"));

            //WHEN
            List<Contacts> contactsDelete = database.Execute<Contacts>(deleteReq, deleteParameters);

            //THEN
            Assert.IsTrue((contactsDelete.Count == 0));
        }
        
        [TestMethod]
        [ExpectedException(typeof(DatabaseException), "An error has occured. Reason :")]
        public void TestExecuteWithWrongSqlStatement()
        {
            //GIVEN
            string deleteReq = @"DELETEZZ FROM contacts WHERE address = @address";
            List<DbParameter> deleteParameters = new List<DbParameter>();
            deleteParameters.Add(new NpgsqlParameter("@address", "ADDRESS CHANGED FOR TEST"));

            //WHEN
            List<Contacts> contactsDelete = database.Execute<Contacts>(deleteReq, deleteParameters);

            //THEN
        }

        [TestMethod]
        [ExpectedException(typeof(DatabaseException), "An error has occured. Reason :")]
        public void TestExecuteWithWrongSelectStatement()
        {
            //GIVEN
            string deleteReq = @"SELECTZZ * FROM contacts WHERE address = @address";
            List<DbParameter> deleteParameters = new List<DbParameter>();
            deleteParameters.Add(new NpgsqlParameter("@address", "ADDRESS CHANGED FOR TEST"));

            //WHEN
            List<Contacts> contactsDelete = database.Execute<Contacts>(deleteReq, deleteParameters);

            //THEN
        }

        [TestMethod]
        [ExpectedException(typeof(WrongSqlRequestException), "Your SQL statement is not a SELECT, INSERT, UPDATE or DELETE statement")]
        public void TestExecuteWithNoSqlStatement()
        {
            //GIVEN
            string deleteReq = String.Empty;

            //WHEN
            List<Contacts> contactsDelete = database.Execute<Contacts>(deleteReq, new List<DbParameter>());

            //THEN
        }
    }
}
