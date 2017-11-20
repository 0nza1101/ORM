using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

using ORMLib.Constants;
using ORMLib.Generics;
using System.Data.Common;

namespace ORMLib.Database
{
    public class MSSql : IDatabase
    {
        private DbConnection _dbConnection;
		private string m_connectionString;

        public string connectionString
		{
            get { return m_connectionString; }
            set { m_connectionString = value; }
		}

        public DbConnection dbConnection
        {
            get
            {
                return this._dbConnection;
            }
            set
            {
                this._dbConnection = value;
            }
        }

        /// <summary>
        ///     Creates a MSSql object by setting its 'connectionString' value
        /// </summary>
        /// <param name="ip">The IP Address of the database. It generally is localhost</param>
        /// <param name="port">Optional Value, needed for MSSql</param>
        /// <param name="dbName">Name of your database</param>
        /// <param name="username">Username to connect to the database</param>
        /// <param name="password">Password to connect to the database</param>
        public MSSql(string ip, string port, string dbName, string username, string password)
        {
            connectionString = String.Format("Network Library=dbmssocn;Data Source={0},{1};Initial Catalog={2};User ID={3};Password={4};",
                                             ip, port, dbName, username, password);
        }
        /// <summary>
        ///     This function executes a SQL 'SELECT' Statement.
        ///     First it initalizes the DbConnection, DbCommand and DbDataReader variables
        ///     Then we execute the Sql Statement with the function 'ExecuteReader' that will return us a DbDataReader that contains the result of our SQL Statement
        ///     We map the object with the values of the DbDataReader and add it to a list with the type of this object
        ///     Finally, we return this list
        /// </summary>
        /// <typeparam name="T">A generic object, can be int, can typeof Contacts</typeparam>
        /// <param name="req">Contains the SQL Statement</param>
        /// <returns>A list that contains the result of the SQL SELECT statement</returns>
        public List<T> Select<T>(string req)
        {
			using (dbConnection = new SqlConnection())
            {
                List<T> list = new List<T>();
                T obj = default(T);

                dbConnection.ConnectionString = connectionString;
                dbConnection.Open();
                DbCommand command = dbConnection.CreateCommand();
                DbDataReader reader;
                try {
                    reader = command.ExecuteReader();
                } catch (Exception e) {
                    throw new ArgumentException("Problème lors de l'exécution de la requête SQL" + e.Message);
                }

                if (reader.HasRows) {
                    while (reader.Read())
                    {
                        obj = Activator.CreateInstance<T>();
                        foreach (PropertyInfo prop in obj.GetType().GetProperties())
                        {
                            if (!reader[prop.Name].Equals(null))
                            {
                                prop.SetValue(obj, reader[prop.Name], null);
                            }
                        }
                        list.Add(obj);
                    }
                }
                else {
                    Console.WriteLine("No rows found.");
                }
                // Always call Close when done reading.
                reader.Close();
                dbConnection.Close();
                dbConnection.Dispose();
                return list;
			}
        }
           
        /// <summary>
        ///     Calling this function would execute the SQL statement that you give this function, with the parameters in the list 'listOfParameters'
        ///     It instaciates a new connection, a SqlCommand object that will allow us to give parameters to our SQL statement
        ///     It also instaciates a DbParamater object that will contain our values of our parameters given in entry in the method
        ///     Then it assigns the values of the parameters to the parameters in the SQL Statement
        ///     Then, depending on SQL Request (SELECT, INSERT, UPDATE, DELETE), it calls either ExecuteNonQuery() or ExecuteReader()
        ///     The difference is that one these functions returns the number of row affected or -1 if 0 rows have been affected and the other one returns a DataReader that contains the result of 
        ///     the SQL Request
        ///     If we call ExecuteNonQuery, we will check if the number of rows affected are superior to 0. If not, we'll throw an exception because no rows were affected by the SQL Statement
        ///     If we call ExecuteReader, we will check if the SQL Statement returned rows. If it did, then we'll create an object with the results of the SQL Statement
        /// </summary>
        /// <typeparam name="T">A generic object, can be int, can typeof Contacts</typeparam>
        /// <param name="req">Contains the SQL statement</param>
        /// <param name="listOfParameters">Contains the value of the parameters in the SQL Statement</param>
        /// <returns>It either returns a list that contains the result of a SELECT Sql Statement, or it returns an empty list</returns>
        public List<T> Execute<T>(string req, List<DbParameter> listOfParameters){
            List<T> list = new List<T>();
            using (dbConnection = new SqlConnection())
            {
                dbConnection.ConnectionString = connectionString;
                DbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandText = req;
                dbCommand.CommandType = CommandType.Text;
                dbCommand.Connection = dbConnection;
                foreach (DbParameter param in listOfParameters)
                {
                    DbParameter sqlParameter = dbCommand.CreateParameter();
                    sqlParameter.ParameterName = param.ParameterName;
                    sqlParameter.Value = param.Value;
                    dbCommand.Parameters.Add(sqlParameter);
                }
                dbConnection.Open();
                int i = 0;
                DbDataReader dbDataReader = null;
                if (req.IndexOf("SELECT") == 0)
                {
                    try
                    {
                        dbDataReader = dbCommand.ExecuteReader();
                    } catch (Exception e)
                    {
                        Console.WriteLine($"Can't perfom operation because : { e.Message }");
                    }
                   
                    T obj = default(T);
                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            obj = Activator.CreateInstance<T>();
                            foreach (PropertyInfo prop in obj.GetType().GetProperties())
                            {
                                if (!dbDataReader[prop.Name].Equals(null))
                                {
                                    prop.SetValue(obj, dbDataReader[prop.Name]);
                                }
                            }
                            list.Add(obj);
                        }
                        dbDataReader.Close();
                    }
                }
                else if(req.IndexOf("INSERT") == 0 
                    || req.IndexOf("UPDATE") == 0 
                    || req.IndexOf("DELETE") == 0)
                {
                    i = dbCommand.ExecuteNonQuery();
                } else {
                    throw new WrongSqlRequestException("Your SQL statement is not a SELECt, INSERT, UPDATE or DELETE statement");
                }

                if(i == -1)
                {
                    throw new NoRowsWereAffectedException("None of the rows in the SQL Statement were affected");
                }

                dbConnection.Close();
                dbConnection.Dispose();
                return list;
            }
        }

    }
}
