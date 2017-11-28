using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORMLib.Constants;
using ORMLib.exception;
using ORMLib.Generics;
using ORMLibTest.Modele;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMLibTest.Generics
{
    [TestClass]
    public class DboMapperTest
    {
        private DboMapper database = new DboMapper("localhost", "1433", "devdb", "sa", "P@55w0rd", DatabaseType.MSSql);

        [TestMethod]
        public void TestListWithSelectStatement()
        {
            //GIVEN
            string selectReq = $"SELECT * FROM contacts";

            //WHEN
            List<Contacts> contactsReturnedBySelect = database.List<Contacts>(selectReq);

            //THEN
            Assert.IsNotNull(contactsReturnedBySelect);
            Assert.IsFalse(contactsReturnedBySelect.Count <= 0);
        }

        [TestMethod]
        [ExpectedException(typeof(WrongSqlRequestException), "Your SQL Statement is not a SELECT Statement. The function List<T> can only be used with a SELECT statement")]
        public void TestListWithStatementOtherThanSelect()
        {
            //GIVEN
            string selectReq = $"DELETE FROM contacts";

            //WHEN
            List<Contacts> contactsReturnedBySelect = database.List<Contacts>(selectReq);

            //THEN
        }

        [TestMethod]
        public void TestExecuteWithSelectStatementAndWithParameters()
        {
            //GIVEN
            string selectReqWithParameters = @"SELECT * FROM contacts WHERE email = @email";
            List<DbParameter> selectParameters = new List<DbParameter>();
            selectParameters.Add(new SqlParameter("@email", "canserkan.uren@gmail.com"));

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
            insertParameters.Add(new SqlParameter("@name", "TEST CASE"));
            insertParameters.Add(new SqlParameter("@address", "RUE DU TEST"));
            insertParameters.Add(new SqlParameter("@email", "TestEmail@gmail.com"));

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
            updateParameters.Add(new SqlParameter("@name", "NAME CHANGED FOR TEST"));
            updateParameters.Add(new SqlParameter("@address", "ADDRESS CHANGED FOR TEST"));
            updateParameters.Add(new SqlParameter("@email", "emailChangedForTest@gmail.com"));
            updateParameters.Add(new SqlParameter("@oldEmail", "TestEmail@gmail.com"));

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
            deleteParameters.Add(new SqlParameter("@address", "ADDRESS CHANGED FOR TEST"));

            //WHEN
            List<Contacts> contactsDelete = database.Execute<Contacts>(deleteReq, deleteParameters);

            //THEN
            Assert.IsTrue((contactsDelete.Count == 0));
        }

        [TestMethod]
        public void TestExecuteWithSelectStatement()
        {
            //GIVEN
            string deleteReq = @"SELECT * FROM contacts";

            //WHEN
            List<Contacts> contactsSelected = database.Execute<Contacts>(deleteReq, new List<DbParameter>());

            //THEN
            Assert.IsNotNull(contactsSelected);
            Assert.IsTrue(contactsSelected.Count > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(RequestIsEmptyException), "You have not provided a SQL statement")]
        public void TestExecuteWithRequestBeingEmptyAndNoParameters()
        {
            //GIVEN
            string deleteReq = String.Empty;

            //WHEN
            database.Execute<Contacts>(deleteReq, new List<DbParameter>());

            //THEN
        }
    }
}
